using System.Linq.Expressions;

namespace OoMapper
{
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