namespace OoMapper
{
    using System.Linq.Expressions;
	using System.Reflection;

    public class PropertyMap
	{
		public PropertyMap(MemberInfo destinationMember, ISourceMemberResolver sourceMemberResolver)
		{
			this.destinationMember = destinationMember;
			this.sourceMemberResolver = sourceMemberResolver;
		}

        private readonly ISourceMemberResolver sourceMemberResolver;

        private readonly MemberInfo destinationMember;

        public Expression BuildAssign(Expression destination, Expression source)
		{
			MemberInfo info = destinationMember;
			return Expression.Assign(Expression.MakeMemberAccess(destination, info),
			                         sourceMemberResolver.BuildSource(source, info.GetMemberType()));
		}

		public MemberAssignment BuildBind(Expression source)
		{
			MemberInfo info = destinationMember;
			return Expression.Bind(info, sourceMemberResolver.BuildSource(source, info.GetMemberType()));
		}
	}
}