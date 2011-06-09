using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OoMapper
{
    public static class TypeMapBuilder
    {
    	public static TypeMap CreateTypeMap(TypeMapConfiguration tmc, IMappingConfiguration configuration)
        {
            IEnumerable<MemberInfo> sourceMembers = GetMembers(tmc.SourceType, true);
            IEnumerable<MemberInfo> destinationMembers = GetMembers(tmc.DestinationType, false);
            List<PropertyMap> propertyMaps = destinationMembers
                .Select(destination => CreatePropertyMap(tmc, configuration, sourceMembers, destination))
                .Where(propertyMap => propertyMap != null)
                .ToList();
            return new TypeMap(tmc.SourceType, tmc.DestinationType, propertyMaps);
        }

        private static PropertyMap CreatePropertyMap(TypeMapConfiguration tmc, IMappingConfiguration configuration, IEnumerable<MemberInfo> sourceMembers, MemberInfo destination)
        {
            PropertyMap propertyMap = null;
            var explicitPropertyMapFound = InheritedConfigurations(tmc, configuration)
                .OrderBy(x => x.SourceType, TypeHierarchyComparer.Instance)
                .ThenBy(x => x.DestinationType, TypeHierarchyComparer.Instance)
                .Any(itmc => MapPropertyMap(itmc, destination, out propertyMap));

            if (explicitPropertyMapFound)
                return propertyMap;

            return new PropertyMap(destination, CreateSourceMemberResolver(destination, sourceMembers, configuration));
        }

        private static IEnumerable<TypeMapConfiguration> InheritedConfigurations(TypeMapConfiguration tmc, IMappingConfiguration configuration)
        {
        	var selectMany = configuration.TypeMapConfigurations.Where(x => x.Including(tmc))
        		.SelectMany(x => InheritedConfigurations(x, configuration));

            yield return tmc;
            foreach (var iitmc in selectMany)
                yield return iitmc;
        }

    	private static bool MapPropertyMap(TypeMapConfiguration tmc, MemberInfo destination, out PropertyMap propertyMap)
        {
            propertyMap = null;
            PropertyMapConfiguration pmc = tmc.GetPropertyMapConfiguration(destination.Name);
            if (pmc == null || !pmc.IsMapped())
                pmc = tmc.GetPropertyMapConfiguration("*");
            if (pmc == null || !pmc.IsMapped())
                return false;
            if (pmc.IsIgnored() == false)
                propertyMap = new PropertyMap(destination, pmc.Resolver);
            return true;
        }

        private static SourceMemberResolver CreateSourceMemberResolver(MemberInfo destination, IEnumerable<MemberInfo> sourceMembers, IMappingConfiguration mappingConfiguration)
        {
            var propertyInfos = new List<MemberInfo>();
        	FindMembers(propertyInfos, destination.Name, sourceMembers);
            return new SourceMemberResolver(propertyInfos, mappingConfiguration);
        }

        private static IEnumerable<MemberInfo> GetMembers(Type sourceType, bool includeMethods)
        {
        	var memberInfos = sourceType.GetProperties()
        		.Concat((MemberInfo[]) sourceType.GetFields());
			if(includeMethods)
        	return memberInfos
        		.Concat(sourceType.GetMethods().Where(x => x.GetParameters().Length == 0));
        	return memberInfos;
        }

    	private static void FindMembers(ICollection<MemberInfo> list, string name, IEnumerable<MemberInfo> sourceMembers)
        {
        	if (String.IsNullOrEmpty(name))
                return;
            MemberInfo memberInfo =
                sourceMembers.FirstOrDefault(pi => name.StartsWith(pi.Name, StringComparison.InvariantCultureIgnoreCase));
            if (memberInfo == null)
                return;
            list.Add(memberInfo);

        	FindMembers(list, name.Substring(memberInfo.Name.Length), GetMembers(memberInfo.GetMemberType(), true));
        }

        private class TypeHierarchyComparer : IComparer<Type>
        {
            public int Compare(Type x, Type y)
            {
                return x == y ? 0 : (x.IsAssignableFrom(y) ? 1 : -1);
            }

            public static readonly TypeHierarchyComparer Instance = new TypeHierarchyComparer();
        }
    }
}