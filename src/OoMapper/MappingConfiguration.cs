using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OoMapper
{
    public interface IMappingConfiguration
    {
        Expression<Func<TSource, TDestination>> BuildNew<TSource, TDestination>();
        LambdaExpression BuildNew(Type sourceType, Type destinationType);
        Expression<Func<TSource, TDestination, TDestination>> BuildExisting<TSource, TDestination>();
        LambdaExpression BuildExisting(Type sourceType, Type destinationType);
        void Reset();
        void AddMapping(TypeMap typeMap);
    }

    public class MappingConfiguration : IMappingConfiguration
    {
        private  readonly IDictionary<Tuple<Type, Type>, TypeMap> mappers =
            new ConcurrentDictionary<Tuple<Type, Type>, TypeMap>();

        private  readonly ConcurrentDictionary<Tuple<Type, Type>, LambdaExpression> newLambdas =
            new ConcurrentDictionary<Tuple<Type, Type>, LambdaExpression>();

        private  readonly ConcurrentDictionary<Tuple<Type, Type>, LambdaExpression> existingLambdas =
            new ConcurrentDictionary<Tuple<Type, Type>, LambdaExpression>();

        public  Expression<Func<TSource, TDestination>> BuildNew<TSource, TDestination>()
        {
            return (Expression<Func<TSource, TDestination>>) BuildNew(typeof (TSource), typeof (TDestination));
        }

        public LambdaExpression BuildNew(Type sourceType, Type destinationType)
        {
            return newLambdas.GetOrAdd(Tuple.Create(sourceType, destinationType), k => mappers[k].BuildNew());
        }

        public  Expression<Func<TSource, TDestination, TDestination>> BuildExisting<TSource, TDestination>()
        {
            return (Expression<Func<TSource, TDestination, TDestination>>) BuildExisting(typeof (TSource), typeof (TDestination));
        }

        public LambdaExpression BuildExisting(Type sourceType, Type destinationType)
        {
            return existingLambdas.GetOrAdd(Tuple.Create(sourceType, destinationType), k => mappers[k].BuildExisting());
        }

        public  void Reset()
        {
            mappers.Clear();
			newLambdas.Clear();
			existingLambdas.Clear();
        }

        public  void AddMapping(TypeMap typeMap)
        {
            mappers.Add(Tuple.Create(typeMap.SourceType, typeMap.DestinationType), typeMap);
        }
    }
}