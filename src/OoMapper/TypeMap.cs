using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
	public class TypeMap
	{
		private readonly IMappingConfiguration configuration;
		private readonly ICollection<PropertyMap> propertyMaps;

		public TypeMap(Type sourceType, Type destinationType, IMappingConfiguration configuration)
		{
			IEnumerable<MemberInfo> sourceMembers = GetMembers(sourceType);

			IEnumerable<MemberInfo> destinationMembers = GetMembers(destinationType);

			SourceType = sourceType;
			DestinationType = destinationType;
			this.configuration = configuration;

			propertyMaps = destinationMembers
				.Select(destination => new PropertyMap(destination, FindMembers(destination, sourceMembers)))
				.ToList();
		}

		public Type SourceType { get; private set; }

		public Type DestinationType { get; private set; }

		private SourceMemberResolver FindMembers(MemberInfo destination, IEnumerable<MemberInfo> sourceMembers)
		{
			var propertyInfos = new List<MemberInfo>();
			FindMembers(propertyInfos, destination.Name, sourceMembers);
			return new SourceMemberResolver(propertyInfos, configuration);
		}

		public LambdaExpression BuildNew()
		{
			const string name = "src";

			ParameterExpression source = Expression.Parameter(SourceType, name);

			MemberAssignment[] bindings = propertyMaps
				.Where(x => x.IsIgnored == false)
				.Select(m => m.BuildBind(source))
				.ToArray();

			return Expression.Lambda(
				Expression.MemberInit(
					Expression.New(DestinationType), bindings), source);
		}

		public LambdaExpression BuildExisting()
		{
			const string name = "src";

			ParameterExpression source = Expression.Parameter(SourceType, name);
			ParameterExpression destination = Expression.Parameter(DestinationType, "dst");

			Expression[] bindings = propertyMaps
				.Where(x => x.IsIgnored == false)
				.Select(m => m.BuildAssign(destination, source))
				.Concat(new[] {destination})
				.ToArray();

			return Expression.Lambda(
				Expression.Block(bindings),
				source,
				destination);
		}

		private static IEnumerable<MemberInfo> GetMembers(Type sourceType)
		{
			return sourceType.GetProperties().Concat((MemberInfo[]) sourceType.GetFields());
		}

		private static void FindMembers(ICollection<MemberInfo> list, string name,
		                                IEnumerable<MemberInfo> sourceMembers)
		{
			if (String.IsNullOrEmpty(name))
				return;
			MemberInfo memberInfo =
				sourceMembers.FirstOrDefault(pi => name.StartsWith(pi.Name, StringComparison.InvariantCultureIgnoreCase));
			if (memberInfo == null)
				return;

			list.Add(memberInfo);

			FindMembers(list, name.Substring(memberInfo.Name.Length), GetMembers(GetType(memberInfo)));
		}

		private static Type GetType(MemberInfo memberInfo)
		{
			if (memberInfo is PropertyInfo)
				return (memberInfo as PropertyInfo).PropertyType;
			if (memberInfo is FieldInfo)
				return (memberInfo as FieldInfo).FieldType;
			throw new NotSupportedException();
		}

		public PropertyMap GetPropertyMapFor(MemberInfo destinationMember)
		{
			return propertyMaps.FirstOrDefault(map => map.DestinationMember == destinationMember);
		}
	}
}