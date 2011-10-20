using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    public static class TypeMapBuilder
    {
        private static readonly MethodInfo[] enumerableExtensions = EnumerableExtensions();

        public static TypeMap CreateTypeMap(TypeMapConfiguration tmc, IUserDefinedConfiguration configuration)
        {
            List<PropertyMap> propertyMaps = GetDestinationMembers(tmc.DestinationType)
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

            var resolver = CreateSourceMemberResolver(destination.Name, tmc.SourceType);
            if (resolver != null)
                return new PropertyMap(destination, resolver);
            return null;
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

        private static ISourceMemberResolver CreateSourceMemberResolver(string destination, Type sourceType)
        {
            if (sourceType.IsDictionary())
                return new DictionarySourceMemberResolver(destination);

            var members = new HashSet<MemberInfo>();
            var findMembers = FindMembers(members, destination, sourceType);
            if (findMembers == false)
                return null;

            ISourceMemberResolver[] resolvers = members
                .Select(x => (ISourceMemberResolver) new SourceMemberResolver(x))
                .Concat(new ISourceMemberResolver[] {new ConvertSourceMemberResolver()})
                .ToArray();

            return new CompositeSourceMemberResolver(resolvers);
        }

        private static IEnumerable<MemberInfo> GetDestinationMembers(Type destinationType)
        {
            foreach (PropertyInfo propertyInfo in destinationType.GetProperties().Where(propertyInfo => propertyInfo.CanWrite))
                yield return propertyInfo;
            foreach (FieldInfo fieldInfo in destinationType.GetFields())
                yield return fieldInfo;
        }

        private static IEnumerable<MemberInfo> GetSourceMembers(Type sourceType)
        {
            foreach (PropertyInfo property in sourceType.GetProperties())
                yield return property;
            foreach (FieldInfo fieldInfo in sourceType.GetFields())
                yield return fieldInfo;
            foreach (MethodInfo methodInfo in sourceType.GetMethods().Where(methodInfo => methodInfo.GetParameters().Length == 0))
                yield return methodInfo;
        }

        private static bool FindMembers(ICollection<MemberInfo> members, string name, Type sourceType)
        {
            if (String.IsNullOrEmpty(name))
                return true;

            IEnumerable<MemberInfo> sourceMembers = GetSourceMembers(sourceType);

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
                return false;
            members.Add(memberInfo);

            return FindMembers(members, name.Substring(memberInfo.Name.Length), memberInfo.GetMemberType());
        }

        private static MethodInfo[] EnumerableExtensions()
        {
            return typeof (Enumerable).GetMethods()
                .Where(x => x.GetParameters().Length == 1)
                .ToArray();
        }

        #region Nested type: TypeHierarchyComparer

        private class TypeHierarchyComparer : IComparer<Type>
        {
            public static readonly TypeHierarchyComparer Instance = new TypeHierarchyComparer();

            #region IComparer<Type> Members

            public int Compare(Type x, Type y)
            {
                return x == y ? 0 : (x.IsAssignableFrom(y) ? 1 : -1);
            }

            #endregion
        }

        #endregion
    }

    internal class DictionarySourceMemberResolver : ISourceMemberResolver
    {
        private readonly string destination;

        public DictionarySourceMemberResolver(string destination)
        {
            this.destination = destination;
        }

        public Expression BuildSource(Expression x, Type destinationType, IMappingConfiguration mappingConfiguration)
        {
            Expression<Func<IDictionary<object, string>, object>> f = (y) => y[destination];
            return f;
        }
    }
}