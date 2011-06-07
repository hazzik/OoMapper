using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OoMapper
{
	public class TypeMap : ITypePair
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
				.Select(destination => new PropertyMap(destination, CreateSourceMemberResolver(destination, sourceMembers)))
				.ToList();
		}

		public Type SourceType { get; private set; }

		public Type DestinationType { get; private set; }

	    public IEnumerable<PropertyMap> PropertyMaps
	    {
	        get { return propertyMaps; }
	    }

		public IEnumerable<Tuple<Type, Type>> Includes
		{
			get { return includes; }
		}

		private SourceMemberResolver CreateSourceMemberResolver(MemberInfo destination, IEnumerable<MemberInfo> sourceMembers)
		{
			var propertyInfos = new List<MemberInfo>();
			FindMembers(propertyInfos, destination.Name, sourceMembers);
			return new SourceMemberResolver(propertyInfos, configuration);
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
		    return PropertyMaps.FirstOrDefault(map => map.DestinationMember.Name.Equals(destinationMember.Name, StringComparison.Ordinal));
		}

		private readonly ICollection<Tuple<Type, Type>> includes = new List<Tuple<Type, Type>>();

		public void Include<TSource, TDestination>()
		{
			includes.Add(Tuple.Create(typeof (TSource), typeof (TDestination)));
		}
	}
}