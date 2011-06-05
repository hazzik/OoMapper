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

        public virtual Expression BuildSource(Expression x, Type destinationType)
        {
            Expression expression = BuildSourceCore(x);
            return configuration.BuildNewExpressionBody(expression, destinationType);
        }

        protected abstract Expression BuildSourceCore(Expression x);
    }

    public class ParameterRewriter : ExpressionVisitor
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