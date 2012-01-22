namespace OoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class MappingConfiguration : IMappingConfiguration
    {
        private readonly IObjectMapperBuilder existingObjectMapperBuilder = new CachedObjectMapperBuilder(new ExistingObjectMapperBuilder());
        private readonly ICache<Tuple<Type, Type>, TypeMap> mappers = new Cache<Tuple<Type, Type>, TypeMap>();
        private readonly ISourceMemberResolver[] resolvers;
        private readonly IUserDefinedConfiguration userDefinedConfiguration = new UserDefinedConfiguration();

        public MappingConfiguration()
        {
            resolvers = new ISourceMemberResolver[]
                            {
                                new SameTypeMapper(),
                                new ConvertibleTypeMapper(),
                                new UserConfigurationMapper(userDefinedConfiguration),
                                new DictionaryToDictionaryMapper(),
                                new EnumerableToEnumerableMapper(),
                                new ObjectToStringMapper(),
                                new ConvertMapper(),
                            };
        }

        #region IMappingConfiguration Members

        public Expression BuildSource(Expression expression, Type destinationType, IMappingConfiguration cfg)
        {
            var sources = resolvers
                .Select(r => r.BuildSource(expression, destinationType, this))
                .Where(s => s != null);

            foreach (var buildSource in sources)
                return buildSource;

            throw new KeyNotFoundException(Tuple.Create(expression.Type, destinationType).ToString());
        }

        public LambdaExpression BuildNew(Type sourceType, Type destinationType)
        {
            var source = Expression.Parameter(sourceType, "src");
            return Expression.Lambda(BuildSource(source, destinationType, this), source);
        }

        public LambdaExpression BuildExisting(Type sourceType, Type destinationType)
        {
            return existingObjectMapperBuilder.Build(GetTypeMap(sourceType, destinationType), this);
        }

        #endregion

        public void AddTypeMapConfiguration(ITypeMapConfiguration tmc)
        {
            userDefinedConfiguration.AddTypeMapConfiguration(tmc);
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
    }
}
