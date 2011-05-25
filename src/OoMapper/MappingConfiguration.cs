using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OoMapper
{
    public interface IMappingConfiguration
    {
        Expression<Func<TSource, TDestination>> BuildNew<TSource, TDestination>();
        LambdaExpression BuildNew(Tuple<Type, Type> key);
        Expression<Func<TSource, TDestination, TDestination>> BuildExisting<TSource, TDestination>();
        LambdaExpression BuildExisting(Tuple<Type, Type> key);
        void Reset();
        void AddMapping(TypeMap typeMap, Tuple<Type, Type> key);
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
            Type sourceType = typeof (TSource);
            Type destinationType = typeof (TDestination);
            Tuple<Type, Type> key = Tuple.Create(sourceType, destinationType);
            return (Expression<Func<TSource, TDestination>>) BuildNew(key);
        }

        public  LambdaExpression BuildNew(Tuple<Type, Type> key)
        {
            return newLambdas.GetOrAdd(key, k => mappers[k].BuildNew());
        }

        public  Expression<Func<TSource, TDestination, TDestination>> BuildExisting<TSource, TDestination>()
        {
            Type sourceType = typeof (TSource);
            Type destinationType = typeof (TDestination);
            Tuple<Type, Type> key = Tuple.Create(sourceType, destinationType);
            return (Expression<Func<TSource, TDestination, TDestination>>) BuildExisting(key);
        }

        public  LambdaExpression BuildExisting(Tuple<Type, Type> key)
        {
            return existingLambdas.GetOrAdd(key, k => mappers[k].BuildExisting());
        }

        public  void Reset()
        {
            mappers.Clear();
        }

        public  void AddMapping(TypeMap typeMap, Tuple<Type, Type> key)
        {
            mappers.Add(key, typeMap);
        }
    }
}