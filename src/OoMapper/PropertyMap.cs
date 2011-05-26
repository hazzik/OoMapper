namespace OoMapper
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;

	public class PropertyMap
	{
		public PropertyMap(MemberInfo destinationProperty, ISourceMemberResolver sourceMemberResolver)
		{
			DestinationMember = destinationProperty;
			SourceMemberResolver = sourceMemberResolver;
		}

		public ISourceMemberResolver SourceMemberResolver { get; set; }

		public MemberInfo DestinationMember { get; private set; }
		
		public bool IsIgnored { get; set; }

		public Expression BuildAssign(Expression destination, Expression source)
		{
			MemberInfo info = DestinationMember;
			return Expression.Assign(Expression.MakeMemberAccess(destination, info),
			                         SourceMemberResolver.BuildSource(source, GetType(info)));
		}

		public MemberAssignment BuildBind(Expression source)
		{
			MemberInfo info = DestinationMember;
			return Expression.Bind(info, SourceMemberResolver.BuildSource(source, GetType(info)));
		}

		private static Type GetType(MemberInfo memberInfo)
		{
			if (memberInfo is PropertyInfo)
				return (memberInfo as PropertyInfo).PropertyType;
			if (memberInfo is FieldInfo)
				return (memberInfo as FieldInfo).FieldType;
			throw new NotSupportedException();
		}

	}
}