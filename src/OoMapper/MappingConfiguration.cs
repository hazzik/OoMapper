namespace OoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class MappingConfiguration : IMappingConfiguration
    {
        private readonly ICache<Tuple<Type, Type>, Delegate> compilledExisting = new Cache<Tuple<Type, Type>, Delegate>();
        private readonly ICache<Tuple<Type, Type>, Delegate> compilledNew = new Cache<Tuple<Type, Type>, Delegate>();
       
        private readonly IObjectMapperBuilder existingObjectMapperBuilder = new CachedObjectMapperBuilder(new ExistingObjectMapperBuilder());
        private readonly ICache<Tuple<Type, Type>, TypeMap> mappers = new Cache<Tuple<Type, Type>, TypeMap>();
        private readonly ISourceMemberResolver sourceMemberResolver = new ConvertSourceMemberResolver();
        private readonly IUserDefinedConfiguration userDefinedConfiguration = new UserDefinedConfiguration();

        private readonly IEnumerable<ISourceMemberResolver> resolvers;

        public MappingConfiguration()
        {
            resolvers = new ISourceMemberResolver[]
                           {
                               new SameTypeMapper(),
                               new ConvertibleTypeMapper(),
                               new UserConfigurationMapper(userDefinedConfiguration),
                               new DictionaryToDictionaryMapper(),
                               new ObjectToDictionaryMapper(), 
                               new EnumerableToEnumerableMapper(),
                               new ObjectToStringMapper(),
                               new ConvertMapper(),
                           };
        }

        public Expression BuildSource(Expression expression, Type destinationType, IMappingConfiguration cfg)
        {
            var sources = resolvers
                .Select(r => r.BuildSource(expression, destinationType, this))
                .Where(s => s != null);

            foreach (var source in sources)
                return source;

            throw new KeyNotFoundException(Tuple.Create(expression.Type, destinationType).ToString());
        }

        public LambdaExpression BuildNew(Type sourceType, Type destinationType)
        {
            var source = Expression.Parameter(sourceType, "src");
            return Expression.Lambda(sourceMemberResolver.BuildSource(source, destinationType, this), source);
        }

        public LambdaExpression BuildExisting(Type sourceType, Type destinationType)
        {
            return existingObjectMapperBuilder.Build(GetTypeMap(sourceType, destinationType), this);
        }

        public void AddTypeMapConfiguration(ITypeMapConfiguration tmc)
        {
            userDefinedConfiguration.AddTypeMapConfiguration(tmc);
        }

        public Delegate GetCompiledExisting(Type sourceType, Type destinationType)
        {
            return compilledExisting.GetOrAdd(Tuple.Create(sourceType, destinationType),
                                              key => BuildExisting(sourceType, destinationType).Compile());
        }

        public Delegate GetCompiledNew(Type sourceType, Type destinationType)
        {
            return compilledNew.GetOrAdd(Tuple.Create(sourceType, destinationType),
                                         key => BuildNew(sourceType, destinationType).Compile());
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