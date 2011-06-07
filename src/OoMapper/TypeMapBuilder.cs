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
            IEnumerable<MemberInfo> sourceMembers = GetMembers(tmc.SourceType);
            IEnumerable<MemberInfo> destinationMembers = GetMembers(tmc.DestinationType);
            List<PropertyMap> propertyMaps = destinationMembers
                .Select(destination => CreatePropertyMap(tmc, configuration, sourceMembers, destination))
                .Where(propertyMap => propertyMap != null)
                .ToList();
            return new TypeMap(tmc.SourceType, tmc.DestinationType, propertyMaps);
        }

        private static PropertyMap CreatePropertyMap(TypeMapConfiguration tmc, IMappingConfiguration configuration, IEnumerable<MemberInfo> sourceMembers, MemberInfo destination)
        {
            PropertyMap propertyMap;
            if (MapPropertyMap(tmc, destination, out propertyMap) ||
                configuration.TypeMapConfigurations.Where(x => x.Including(tmc)).Any(inheritedTmc => MapPropertyMap(inheritedTmc, destination, out propertyMap)))
            {
                return propertyMap;
            }
            return new PropertyMap(destination, CreateSourceMemberResolver(destination, sourceMembers, configuration));
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

            FindMembers(list, name.Substring(memberInfo.Name.Length), GetMembers(memberInfo.GetMemberType()));
        }
    }
}