using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    public class SourceMemberResolver : SourceMemberResolverBase
    {
		private readonly IEnumerable<MemberInfo> source;

        public SourceMemberResolver(IEnumerable<MemberInfo> source, IMappingConfiguration configuration) 
			: base(configuration)
	    {
		    this.source = source;
	    }

        protected override Expression BuildSourceCore(Expression x)
		{
		    return source.Aggregate(x, GetMemberExpression);
		}

        private static Expression GetMemberExpression(Expression source, MemberInfo sourceProperty)
        {
            if (sourceProperty is PropertyInfo)
                return Expression.Property(source, (PropertyInfo) sourceProperty);
            if (sourceProperty is FieldInfo)
                return Expression.Field(source, (FieldInfo)sourceProperty);
			if (sourceProperty is MethodInfo)
			{
			    var methodInfo = (MethodInfo) sourceProperty;
			    return methodInfo.IsStatic
			               ? Expression.Call(null, methodInfo, new[] {source})
			               : Expression.Call(source, methodInfo);
			}

            throw new NotSupportedException();
        }
    }
}