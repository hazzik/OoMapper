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

        public SourceMemberResolver(List<MemberInfo> source, IMappingConfiguration configuration) 
			: base(configuration)
	    {
		    this.source = source;
	    }

		protected override Expression BuildSourceCore(Expression x)
		{
		    return source.Aggregate(x, GetMemberExpression);
		}

        protected static MemberExpression GetMemberExpression(Expression source, MemberInfo sourceProperty)
        {
            if (sourceProperty is PropertyInfo)
                return Expression.Property(source, (PropertyInfo) sourceProperty);
            if (sourceProperty is FieldInfo)
                return Expression.Field(source, (FieldInfo)sourceProperty);

            throw new NotSupportedException();
        }
    }
}