using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OoMapper
{
    public static class TypeMapBuilder
    {
        public static TypeMap CreateTypeMap(Type sourceType, Type destinationType, IMappingConfiguration configuration)
        {
            IEnumerable<MemberInfo> sourceMembers = GetMembers(sourceType);
            IEnumerable<MemberInfo> destinationMembers = GetMembers(destinationType);
            List<PropertyMap> propertyMaps = destinationMembers
                .Select(destination => new PropertyMap(destination, CreateSourceMemberResolver(destination, sourceMembers, configuration)))
                .ToList();
            return new TypeMap(sourceType, destinationType, propertyMaps);
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