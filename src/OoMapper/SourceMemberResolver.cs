using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    internal sealed class SourceMemberResolver : ISourceMemberResolver
    {
        private readonly MemberInfo memberInfo;

        public SourceMemberResolver(MemberInfo memberInfo)
        {
            this.memberInfo = memberInfo;
        }

        public Expression BuildSource(Expression x, Type destinationType, IMappingConfiguration mappingConfiguration)
        {
            return GetMemberExpression(x, memberInfo);
        }

        private static Expression GetMemberExpression(Expression source, MemberInfo sourceProperty)
        {
            var property = sourceProperty as PropertyInfo;
            if (property != null)
                return Expression.Property(source, property);
            
            var field = sourceProperty as FieldInfo;
            if (field != null)
                return Expression.Field(source, field);
            
            var method = sourceProperty as MethodInfo;
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