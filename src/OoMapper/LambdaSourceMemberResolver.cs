using System.Linq.Expressions;

namespace OoMapper
{
	public class LambdaSourceMemberResolver : SourceMemberResolverBase
    {
        private readonly LambdaExpression sourceMember;

		public LambdaSourceMemberResolver(LambdaExpression sourceMember, IMappingConfiguration configuration)
			:base(configuration)
        {
            this.sourceMember = sourceMember;
        }

	    protected override Expression BuildSourceCore(Expression x)
		{
		    return new ParameterRewriter(sourceMember.Parameters[0], x).Visit(sourceMember.Body);
		}
    }
}