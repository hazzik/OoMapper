using System;
using System.Linq.Expressions;

namespace OoMapper
{
    public abstract class SourceMemberResolverBase : ISourceMemberResolver
    {
        private readonly IMappingConfiguration configuration;

        protected SourceMemberResolverBase(IMappingConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Expression BuildSource(Expression x, Type destinationType)
        {
            Expression expression = BuildSourceCore(x, destinationType);
            Type sourceType = expression.Type;
            if (destinationType == sourceType || destinationType.IsAssignableFrom(sourceType))
            {
                return expression;
            }
            if (destinationType == typeof (string))
            {
                return Expression.Call(expression, "ToString", new Type[0]);
            }
            try
            {
                return Expression.Convert(expression, destinationType);
            }
            catch (InvalidOperationException)
            {
                LambdaExpression lambda = configuration.BuildNew(sourceType, destinationType);
                return new ParameterRewriter(lambda.Parameters[0], expression).Visit(lambda.Body);
            }
        }

        protected abstract Expression BuildSourceCore(Expression x, Type destinationType);

        protected class ParameterRewriter : ExpressionVisitor
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