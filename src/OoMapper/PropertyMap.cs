namespace OoMapper
{
    using System.Linq.Expressions;
	using System.Reflection;

    public class PropertyMap
	{
		public PropertyMap(MemberInfo destinationProperty, ISourceMemberResolver sourceMemberResolver)
		{
			destinationMember = destinationProperty;
			SourceMemberResolver = sourceMemberResolver;
		}

		public ISourceMemberResolver SourceMemberResolver { get; set; }

        private readonly MemberInfo destinationMember;
		
		public bool IsIgnored { get; set; }

		public Expression BuildAssign(Expression destination, Expression source)
		{
			MemberInfo info = destinationMember;
			return Expression.Assign(Expression.MakeMemberAccess(destination, info),
			                         SourceMemberResolver.BuildSource(source, info.GetMemberType()));
		}

		public MemberAssignment BuildBind(Expression source)
		{
			MemberInfo info = destinationMember;
			return Expression.Bind(info, SourceMemberResolver.BuildSource(source, info.GetMemberType()));
		}

        public string DestinationMemberName
        {
            get { return destinationMember.Name; }
        }
	}
}