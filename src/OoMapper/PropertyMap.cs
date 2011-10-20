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

        public Expression BuildAssign(Expression destination, Expression source, IMappingConfiguration configuration)
		{
			MemberInfo info = destinationMember;
			return Expression.Assign(Expression.MakeMemberAccess(destination, info),
			                         sourceMemberResolver.BuildSource(source, info.GetMemberType(), configuration));
		}

		public MemberAssignment BuildBind(Expression source, IMappingConfiguration configuration)
		{
			MemberInfo info = destinationMember;
		    var expression = sourceMemberResolver.BuildSource(source, info.GetMemberType(), configuration);
		    return Expression.Bind(info, expression);
		}
	}
}