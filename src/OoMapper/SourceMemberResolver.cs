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

        public Expression BuildSource(Expression x, Type destinationType, IMappingConfiguration mappingConfiguration, IMappingOptions options)
        {
            return GetMemberExpression(x, memberInfo);
        }

        private static Expression GetMemberExpression(Expression source, MemberInfo sourceProperty)
        {
            if (sourceProperty is PropertyInfo)
                return Expression.Property(source, (PropertyInfo) sourceProperty);
            if (sourceProperty is FieldInfo)
                return Expression.Field(source, (FieldInfo) sourceProperty);
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