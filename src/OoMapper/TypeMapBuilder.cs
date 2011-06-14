using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OoMapper
{
    public static class TypeMapBuilder
    {
        private static readonly MethodInfo[] enumerableExtensions = EnumerableExtensions();

        public static TypeMap CreateTypeMap(TypeMapConfiguration tmc, IUserDefinedConfiguration configuration)
        {
            IEnumerable<MemberInfo> destinationMembers = GetMembers(tmc.DestinationType, false);
            List<PropertyMap> propertyMaps = destinationMembers
                .Select(destination => CreatePropertyMap(tmc, configuration, destination))
                .Where(propertyMap => propertyMap != null)
                .ToList();
            return new TypeMap(tmc.SourceType, tmc.DestinationType, propertyMaps);
        }

        private static PropertyMap CreatePropertyMap(TypeMapConfiguration tmc,
                                                     IUserDefinedConfiguration configuration,
                                                     MemberInfo destination)
        {
            PropertyMap propertyMap = null;
            bool explicitPropertyMapFound = configuration.InheritedConfigurations(tmc)
                .OrderBy(x => x.SourceType, TypeHierarchyComparer.Instance)
                .ThenBy(x => x.DestinationType, TypeHierarchyComparer.Instance)
                .Any(itmc => MapPropertyMap(itmc, destination, out propertyMap));

            if (explicitPropertyMapFound)
                return propertyMap;

            return new PropertyMap(destination, CreateSourceMemberResolver(destination, tmc.SourceType));
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

        private static ISourceMemberResolver CreateSourceMemberResolver(MemberInfo destination, Type sourceType)
        {
            var propertyInfos = new List<MemberInfo>();
            FindMembers(propertyInfos, destination.Name, sourceType);
            ISourceMemberResolver[] list = propertyInfos
                .Select(x => new SourceMemberResolver(x))
                .Concat(new ISourceMemberResolver[] {new ConvertSourceMemberResolver()})
                .ToArray();
            return new CompositeSourceMemberResolver(list);
        }

        private static IEnumerable<MemberInfo> GetMembers(Type sourceType, bool includeMethods)
        {
            IEnumerable<MemberInfo> memberInfos = sourceType.GetProperties()
                .Concat((MemberInfo[]) sourceType.GetFields());
            if (includeMethods)
                return memberInfos.Concat(sourceType.GetMethods().Where(x => x.GetParameters().Length == 0));
            return memberInfos;
        }

        private static void FindMembers(ICollection<MemberInfo> list, string name, Type sourceType)
        {
            if (String.IsNullOrEmpty(name))
                return;

            var sourceMembers = GetMembers(sourceType, true);

            MemberInfo memberInfo =
                sourceMembers.FirstOrDefault(pi => name.StartsWith(pi.Name, StringComparison.InvariantCultureIgnoreCase));
            if (memberInfo == null && sourceType.IsEnumerable())
            {
                Type sourceElementType = TypeUtils.GetElementTypeOfEnumerable(sourceType);
                memberInfo = enumerableExtensions
                    .Where(x => x.Name == name)
                    .Select(x => x.IsGenericMethodDefinition
                                     ? x.MakeGenericMethod(sourceElementType)
                                     : x)
                    .Where(x => TypeUtils.GetElementTypeOfEnumerable(x.GetParameters().First().ParameterType) == sourceElementType)
                    .FirstOrDefault();
            }
            if (memberInfo == null)
                return;
            list.Add(memberInfo);

            FindMembers(list, name.Substring(memberInfo.Name.Length), memberInfo.GetMemberType());
        }

        private static MethodInfo[] EnumerableExtensions()
        {
            return typeof (Enumerable).GetMethods()
                .Where(x => x.GetParameters().Length == 1)
                .ToArray();
        }

        private class TypeHierarchyComparer : IComparer<Type>
        {
            public static readonly TypeHierarchyComparer Instance = new TypeHierarchyComparer();

            public int Compare(Type x, Type y)
            {
                return x == y ? 0 : (x.IsAssignableFrom(y) ? 1 : -1);
            }
        }
    }
}