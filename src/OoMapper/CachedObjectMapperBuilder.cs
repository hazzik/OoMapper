using System;
using System.Linq.Expressions;

namespace OoMapper
{
    public class CachedObjectMapperBuilder : IObjectMapperBuilder
    {
        private readonly IObjectMapperBuilder inner;

        private readonly ICache<Tuple<Type, Type>, LambdaExpression> concurrentDictionary =
            new Cache<Tuple<Type, Type>, LambdaExpression>();

        public CachedObjectMapperBuilder(IObjectMapperBuilder inner)
        {
            this.inner = inner;
        }

        public LambdaExpression Build(TypeMap typeMap, IMappingConfiguration configuration)
        {
            var tuple = Tuple.Create(typeMap.SourceType, typeMap.DestinationType);
            return concurrentDictionary.GetOrAdd(tuple, k => inner.Build(typeMap, configuration));
        }
    }
}