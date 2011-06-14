using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    public class SourceMemberResolver : SourceMemberResolverBase
    {
		private readonly IEnumerable<MemberInfo> source;

        public SourceMemberResolver(IEnumerable<MemberInfo> source)
        {
		    this.source = source;
	    }

        protected override Expression BuildSourceCore(Expression x)
        {
            var expression = x;
            Expression condition = Expression.Constant(false);
            foreach (var memberInfo in source)
            {
                if (expression.Type.IsValueType == false)
                    condition = Expression.OrElse(condition, Expression.Equal(expression, Expression.Constant(null)));
                expression = GetMemberExpression(expression, memberInfo);
            }
            return Expression.Condition(condition, Expression.Default(expression.Type), expression, expression.Type);
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