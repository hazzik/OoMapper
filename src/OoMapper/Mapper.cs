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

    	private static readonly ConcurrentDictionary<Tuple<Type, Type>, LambdaExpression> newLambdas =
			new ConcurrentDictionary<Tuple<Type, Type>, LambdaExpression>();

    	private static readonly ConcurrentDictionary<Tuple<Type, Type>, LambdaExpression> existingLambdas =
			new ConcurrentDictionary<Tuple<Type, Type>, LambdaExpression>();
		
        public static void CreateMap<TSource, TDestination>()
        {
            Type sourceType = typeof (TSource);
            Type destinationType = typeof (TDestination);

            var typeMap = new TypeMap(sourceType, destinationType);
            
            Tuple<Type, Type> key = Tuple.Create(sourceType, destinationType);

            mappers.Add(key, typeMap);
        }

    	public static TDestination Map<TSource, TDestination>(TSource source)
        {
        	return BuildNew<TSource, TDestination>().Compile().Invoke(source);
        }

    	public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    	{
    		return BuildExisting<TSource, TDestination>().Compile().Invoke(source, destination);
    	}

    	public static Expression<Func<TSource, TDestination>> BuildNew<TSource, TDestination>()
    	{
			Type sourceType = typeof(TSource);
			Type destinationType = typeof(TDestination);
    		Tuple<Type, Type> key = Tuple.Create(sourceType, destinationType);
			return (Expression<Func<TSource, TDestination>>)BuildNew(key);
    	}

    	private static LambdaExpression BuildNew(Tuple<Type, Type> key)
    	{
    		return newLambdas.GetOrAdd(key, k => mappers[k].BuildNew());
    	}

    	private static Expression<Func<TSource, TDestination, TDestination>> BuildExisting<TSource, TDestination>()
    	{
    		Type sourceType = typeof (TSource);
    		Type destinationType = typeof (TDestination);
    		Tuple<Type, Type> key = Tuple.Create(sourceType, destinationType);
    		return (Expression<Func<TSource, TDestination, TDestination>>)BuildExisting(key);
    	}

    	private static LambdaExpression BuildExisting(Tuple<Type, Type> key)
    	{
			return existingLambdas.GetOrAdd(key, k => mappers[k].BuildExisting());
    	}

    	public static void Reset()
        {
            mappers.Clear();
        }
    }
}