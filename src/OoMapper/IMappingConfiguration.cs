using System;
using System.Linq.Expressions;

namespace OoMapper
{
    public interface IMappingConfiguration
    {
        Expression<Func<TSource, TDestination>> BuildNew<TSource, TDestination>();
        LambdaExpression BuildNew(Type sourceType, Type destinationType);
        Expression<Func<TSource, TDestination, TDestination>> BuildExisting<TSource, TDestination>();
        LambdaExpression BuildExisting(Type sourceType, Type destinationType);
        void AddMapping(TypeMap typeMap);
    }
}