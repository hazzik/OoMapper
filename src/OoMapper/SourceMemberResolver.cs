using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    public class SourceMemberResolver : SourceMemberResolverBase
    {
		private readonly List<MemberInfo> source;
	    private readonly IMappingConfiguration configuration;

	    public SourceMemberResolver(List<MemberInfo> source, IMappingConfiguration configuration) 
			: base(configuration)
	    {
		    this.source = source;
		    this.configuration = configuration;
		}

		protected override Expression BuildSourceCore(Expression x, Type destinationType)
		{
			return source.Aggregate(x, (current, memberInfo) =>
			                           CreatePropertyExpression(current, memberInfo, destinationType));
		}

		private Expression CreatePropertyExpression(Expression x, MemberInfo sourceProperty, Type destinationType)
		{
			MemberExpression property = GetMemberExpression(x, sourceProperty);

			if (destinationType.IsEnumerable())
			{
				var isArray = destinationType.IsArray;
				destinationType = TypeUtils.GetElementType(destinationType);
				Type sourceType = TypeUtils.GetElementType(property.Type);
				if (sourceType == null) return property;
				return CreateSelect(sourceType, destinationType, property, isArray ? "ToArray" : "ToList");
			}

			return property;
		}

    	private static MemberExpression GetMemberExpression(Expression source, MemberInfo sourceProperty)
    	{
			if (sourceProperty is PropertyInfo)
				return Expression.Property(source, (PropertyInfo) sourceProperty);
			if (sourceProperty is FieldInfo)
				return Expression.Field(source, (FieldInfo)sourceProperty);

    		throw new NotSupportedException();
    	}

	    private Expression CreateSelect(Type sourceType, Type destinationType, Expression property, string methodName)
		{
		    return Expression.Call(typeof (Enumerable), methodName, new[] {destinationType},
			                       Expression.Call(typeof (Enumerable), "Select",
			                                       new[] {sourceType, destinationType},
                                                   property, configuration.BuildNew(sourceType, destinationType)));
		}
	}
}