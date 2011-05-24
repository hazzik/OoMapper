using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OoMapper
{
    public static class Mapper
    {
        public static readonly IDictionary<Tuple<Type, Type>, TypeMap> mappers =
            new ConcurrentDictionary<Tuple<Type, Type>, TypeMap>();

        public static void Configure<TSource, TDestination>()
        {
            Type sourceType = typeof (TSource);
            Type destinationType = typeof (TDestination);

            var typeMap = new TypeMap(sourceType, destinationType);
            
            Tuple<Type, Type> key = Tuple.Create(sourceType, destinationType);

            mappers.Add(key, typeMap);
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            Type sourceType = typeof (TSource);
            Type destinationType = typeof (TDestination);
            Tuple<Type, Type> key = Tuple.Create(sourceType, destinationType);
            var expression = (Expression<Func<TSource, TDestination>>)mappers[key].BuildNew();
            return expression.Compile().Invoke(source);
        }
       
        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            Type sourceType = typeof (TSource);
            Type destinationType = typeof (TDestination);
            Tuple<Type, Type> key = Tuple.Create(sourceType, destinationType);
            var expression = (Expression<Func<TSource, TDestination, TDestination>>)mappers[key].BuildExisting();
            return expression.Compile().Invoke(source, destination);
        }

        public static void Reset()
        {
            mappers.Clear();
        }
    }
}