using System;
using System.Linq.Expressions;

namespace OoMapper
{
    public class LambdaSourceMemberResolver : ISourceMemberResolver
    {
        private readonly LambdaExpression sourceMember;

        public LambdaSourceMemberResolver(LambdaExpression sourceMember)
        {
            this.sourceMember = sourceMember;
        }

        public Expression BuildSource(Expression x, Type destinationType)
        {
            return new ParameterRewriter(sourceMember.Parameters[0], x).Visit(sourceMember.Body);
        }

        private class ParameterRewriter : ExpressionVisitor
        {
            private readonly Expression candidate;
            private readonly Expression replacement;

            public ParameterRewriter(Expression candidate, Expression replacement)
            {
                this.candidate = candidate;
                this.replacement = replacement;
            }

            public override Expression Visit(Expression node)
            {
                return node == candidate ? replacement : base.Visit(node);
            }
        }
    }
}