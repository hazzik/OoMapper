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
            return ExpressionEx.Member(x, memberInfo);
        }
    }
}