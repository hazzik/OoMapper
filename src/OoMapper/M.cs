using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
	public class M
	{
		private readonly PropertyInfo destinationProperty;
		private readonly SourceMemberResolver sourceMemberResolver;

		public M(SourceMemberResolver sourceMemberResolver, PropertyInfo destinationProperty)
		{
			this.destinationProperty = destinationProperty;
			this.sourceMemberResolver = sourceMemberResolver;
		}

		public Expression BuildAssign(Expression destination, Expression source)
		{
			PropertyInfo info = destinationProperty;
			return Expression.Assign(Expression.MakeMemberAccess(destination, info), sourceMemberResolver.BuildSource(source, info.PropertyType));
		}

		public MemberAssignment BuildBind(Expression source)
		{
			PropertyInfo info = destinationProperty;
			return Expression.Bind(info, sourceMemberResolver.BuildSource(source, info.PropertyType));
		}
	}
}