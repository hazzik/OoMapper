using System;
using System.Linq.Expressions;

namespace OoMapper
{
    public class LambdaSourceMemberResolver : ISourceMemberResolver
    {
        private readonly LambdaExpression sourceMember;
        private readonly IMappingConfiguration configuration;

        public LambdaSourceMemberResolver(LambdaExpression sourceMember, IMappingConfiguration configuration)
        {
            this.sourceMember = sourceMember;
            this.configuration = configuration;
        }

        public Expression BuildSource(Expression x, Type destinationType)
        {
            Expression expression = new ParameterRewriter(sourceMember.Parameters[0], x).Visit(sourceMember.Body);

            if(expression.Type != destinationType)
            {
                LambdaExpression lambda = configuration.BuildNew(expression.Type, destinationType);
                return new ParameterRewriter(lambda.Parameters[0], expression).Visit(lambda.Body);
            }

            return expression;
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