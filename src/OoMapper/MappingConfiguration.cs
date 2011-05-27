using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OoMapper
{
    public class MappingConfiguration : IMappingConfiguration
    {
        private readonly IDictionary<Tuple<Type, Type>, TypeMap> mappers =
            new ConcurrentDictionary<Tuple<Type, Type>, TypeMap>();

        private readonly IObjectMapperBuilder newObjectMapperBuilder = new CachedObjectMapperBuilder(new NewObjectMapperBuilder());
        private readonly IObjectMapperBuilder existingObjectMapperBuilder = new CachedObjectMapperBuilder(new ExistingObjectMapperBuilder());

        public  Expression<Func<TSource, TDestination>> BuildNew<TSource, TDestination>()
        {
            return (Expression<Func<TSource, TDestination>>) BuildNew(typeof (TSource), typeof (TDestination));
        }

        public LambdaExpression BuildNew(Type sourceType, Type destinationType)
        {
            Tuple<Type, Type> tuple = Tuple.Create(sourceType, destinationType);
            return newObjectMapperBuilder.Build(mappers[tuple]);
        }

        public  Expression<Func<TSource, TDestination, TDestination>> BuildExisting<TSource, TDestination>()
        {
            return (Expression<Func<TSource, TDestination, TDestination>>) BuildExisting(typeof (TSource), typeof (TDestination));
        }

        public LambdaExpression BuildExisting(Type sourceType, Type destinationType)
        {
            Tuple<Type, Type> tuple = Tuple.Create(sourceType, destinationType);
            return existingObjectMapperBuilder.Build(mappers[tuple]);
        }

        public  void AddMapping(TypeMap typeMap)
        {
            mappers.Add(Tuple.Create(typeMap.SourceType, typeMap.DestinationType), typeMap);
        }
    }
}