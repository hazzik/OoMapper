namespace OoMapper
{
	using System;
	using System.Linq.Expressions;

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
		    try
		    {
		        return Expression.Convert(expression, destinationType);
		    }
		    catch (InvalidOperationException)
		    {
		        LambdaExpression lambda = configuration.BuildNew(expression.Type, destinationType);
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