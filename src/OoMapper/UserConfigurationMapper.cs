namespace OoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class UserConfigurationMapper : ISourceMemberResolver
    {
        private readonly IDictionary<ITypeMapConfiguration, DynamicMapperBase> instances = new Dictionary<ITypeMapConfiguration, DynamicMapperBase>();
        private readonly Lazy<DynamicMapperBuilder> lazyDynamicMapperBuilder = new Lazy<DynamicMapperBuilder>(DynamicMapperBuilder.Create);
        private readonly ICache<Tuple<Type, Type>, TypeMap> mappers = new Cache<Tuple<Type, Type>, TypeMap>();
        private readonly IUserDefinedConfiguration userDefinedConfiguration;

        public UserConfigurationMapper(IUserDefinedConfiguration userDefinedConfiguration)
        {
            this.userDefinedConfiguration = userDefinedConfiguration;
        }

        private DynamicMapperBuilder DynamicMapperBuilder
        {
            get { return lazyDynamicMapperBuilder.Value; }
        }

        #region ISourceMemberResolver Members

        public Expression BuildSource(Expression expression, Type destinationType, IMappingConfiguration mappingConfiguration)
        {
            var sourceType = expression.Type;
            var map = userDefinedConfiguration.FindTypeMapConfiguration(sourceType, destinationType);
            if (map == null)
                return null;

            Expression<Func<DynamicMapperBase>> mapper = () => GetDynamicMapper(map, mappingConfiguration);
            return Expression.Convert(Expression.Call(mapper.Body, "DynamicMap", Type.EmptyTypes, expression), destinationType);
        }

        #endregion

        private TypeMap CreateOrGetTypeMap(ITypeMapConfiguration map)
        {
            return mappers.GetOrAdd(Tuple.Create(map.SourceType, map.DestinationType), k => TypeMapBuilder.CreateTypeMap(map, userDefinedConfiguration));
        }

        private DynamicMapperBase GetDynamicMapper(ITypeMapConfiguration map, IMappingConfiguration mappingConfiguration)
        {
            DynamicMapperBase res;
            if (instances.TryGetValue(map, out res))
                return res;

            var typeMaps = GetTypeMapsWithIncludes(map).ToArray();
            var dynamicMapper = DynamicMapperBuilder.BuildDynamicMapperType(typeMaps);
            var dynamicMapperBase = (DynamicMapperBase) Activator.CreateInstance(dynamicMapper, mappingConfiguration);
            instances.Add(map, dynamicMapperBase);
            return dynamicMapperBase;
        }

        private ITypeMapConfiguration GetTypeMapConfiguration(Tuple<Type, Type> include)
        {
            var sourceType = include.Item1;
            var destinationType = include.Item2;
            var map = userDefinedConfiguration.FindTypeMapConfiguration(sourceType, destinationType);
            if (map == null) throw new KeyNotFoundException(include.ToString());
            return map;
        }

        private IEnumerable<TypeMap> GetTypeMapsWithIncludes(ITypeMapConfiguration map)
        {
            yield return CreateOrGetTypeMap(map);
            foreach (var include in map.Includes.SelectMany(x => GetTypeMapsWithIncludes(GetTypeMapConfiguration(x))))
                yield return include;
        }
    }
}
