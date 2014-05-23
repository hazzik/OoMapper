using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    internal static class ExpressionEx
    {
        public static Expression Member(Expression source, MemberInfo member)
        {
            var property = member as PropertyInfo;
            if (property != null)
                return Expression.Property(source, property);
            var field = member as FieldInfo;
            if (field != null)
                return Expression.Field(source, field);
            var method = member as MethodInfo;
            if (method != null)
            {
                return method.IsStatic
                    ? Expression.Call(null, method, new[] {source})
                    : Expression.Call(source, method);
            }

            throw new NotSupportedException();
        }
    }
}