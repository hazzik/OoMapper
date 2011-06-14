using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace OoMapper
{
    public class CachedObjectMapperBuilder : IObjectMapperBuilder
    {
        private readonly IObjectMapperBuilder inner;

        private readonly ConcurrentDictionary<Tuple<Type, Type>, LambdaExpression> concurrentDictionary =
            new ConcurrentDictionary<Tuple<Type, Type>, LambdaExpression>();

        public CachedObjectMapperBuilder(IObjectMapperBuilder inner)
        {
            this.inner = inner;
        }

        public LambdaExpression Build(TypeMap typeMap, IMappingConfiguration configuration, IMappingOptions options)
        {
            var tuple = Tuple.Create(typeMap.SourceType, typeMap.DestinationType);
            return concurrentDictionary.GetOrAdd(tuple, k => inner.Build(typeMap, configuration, options));
        }
    }
}