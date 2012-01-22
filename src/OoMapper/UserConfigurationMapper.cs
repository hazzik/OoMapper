namespace OoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal class UserConfigurationMapper : ISourceMemberResolver
    {
        private readonly Lazy<DynamicMapperBuilder> lazyDynamicMapperBuilder = new Lazy<DynamicMapperBuilder>(DynamicMapperBuilder.Create);
        private readonly ICache<Tuple<Type, Type>, TypeMap> mappers = new Cache<Tuple<Type, Type>, TypeMap>();
        private readonly IObjectMapperBuilder newObjectMapperBuilder = new CachedObjectMapperBuilder(new NewObjectMapperBuilder());
        private readonly ISet<ITypeMapConfiguration> processed = new HashSet<ITypeMapConfiguration>();
        private readonly IUserDefinedConfiguration userDefinedConfiguration;

        public UserConfigurationMapper(IUserDefinedConfiguration userDefinedConfiguration)
        {
            this.userDefinedConfiguration = userDefinedConfiguration;
        }

        private DynamicMapperBuilder DynamicMapperBuilder
        {
            get { return lazyDynamicMapperBuilder.Value; }
        }

        public Expression BuildSource(Expression expression, Type destinationType, IMappingConfiguration mappingConfiguration)
        {
            var map = userDefinedConfiguration.FindTypeMapConfiguration(expression.Type, destinationType);
            if (map == null)
                return null;

            if (processed.Add(map) == false || map.HasIncludes() == false)
            {
                var typeMap = CreateOrGetTypeMap(map);
                var lambda = newObjectMapperBuilder.Build(typeMap, mappingConfiguration);
                return new ParameterRewriter(lambda.Parameters[0], expression).Visit(lambda.Body);
            }

            var typeMaps = GetTypeMapsWithIncludes(map).ToArray();
            var dynamicMapper = DynamicMapperBuilder.BuildDynamicMapperType(typeMaps);
            var instance = Activator.CreateInstance(dynamicMapper, mappingConfiguration);
            return Expression.Convert(Expression.Call(Expression.Constant(instance), "DynamicMap", Type.EmptyTypes, expression), destinationType);
        }

        private TypeMap CreateOrGetTypeMap(ITypeMapConfiguration map)
        {
            return mappers.GetOrAdd(Tuple.Create(map.SourceType, map.DestinationType), k => TypeMapBuilder.CreateTypeMap(map, userDefinedConfiguration));
        }

        private TypeMap GetTypeMap(Type sourceType, Type destinationType)
        {
            var map = userDefinedConfiguration.FindTypeMapConfiguration(sourceType, destinationType);
            if (map == null) throw new KeyNotFoundException(Tuple.Create(sourceType, destinationType).ToString());
            return CreateOrGetTypeMap(map);
        }

        private IEnumerable<TypeMap> GetTypeMapsWithIncludes(ITypeMapConfiguration map)
        {
            yield return CreateOrGetTypeMap(map);
            foreach (var include in map.Includes)
                yield return GetTypeMap(include.Item1, include.Item2);
        }
    }
}