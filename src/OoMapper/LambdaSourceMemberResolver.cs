using System;
using System.Linq.Expressions;

namespace OoMapper
{
	public sealed class LambdaSourceMemberResolver : ISourceMemberResolver
	{
        private readonly LambdaExpression sourceMember;

		public LambdaSourceMemberResolver(LambdaExpression sourceMember)
		{
            this.sourceMember = sourceMember;
        }

	    public Expression BuildSource(Expression x, Type destinationType, IMappingConfiguration mappingConfiguration)
	    {
	        return new ParameterRewriter(sourceMember.Parameters[0], x).Visit(sourceMember.Body);
	    }
    }
}