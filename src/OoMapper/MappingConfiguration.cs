using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OoMapper
{
    public class MappingConfiguration : IMappingConfiguration
    {
        private readonly IObjectMapperBuilder existingObjectMapperBuilder = new CachedObjectMapperBuilder(new ExistingObjectMapperBuilder());

        private readonly IDictionary<Tuple<Type, Type>, TypeMap> mappers =
            new ConcurrentDictionary<Tuple<Type, Type>, TypeMap>();

        private readonly IObjectMapperBuilder newObjectMapperBuilder = new CachedObjectMapperBuilder(new NewObjectMapperBuilder());

        #region IMappingConfiguration Members

        public Expression<Func<TSource, TDestination>> BuildNew<TSource, TDestination>()
        {
            return (Expression<Func<TSource, TDestination>>) BuildNew(typeof (TSource), typeof (TDestination));
        }

        public LambdaExpression BuildNew(Type sourceType, Type destinationType)
        {
            if (sourceType.IsEnumerable() && destinationType.IsEnumerable())
            {
                Type sourceElementType = TypeUtils.GetElementType(sourceType);
                Type destinationElementType = TypeUtils.GetElementType(destinationType);
                var parameterExpression = Expression.Parameter(sourceType, "src");
                return Expression.Lambda(Expression.Convert(CreateSelect(sourceElementType, destinationElementType, parameterExpression, "ToList"), destinationType), parameterExpression);
            }
            return newObjectMapperBuilder.Build(GetTypeMap(Tuple.Create(sourceType, destinationType)));
        }
        private Expression CreateSelect(Type sourceType, Type destinationType, Expression property, string methodName)
        {
            return Expression.Call(typeof(Enumerable), methodName, new[] { destinationType },
                                   Expression.Call(typeof(Enumerable), "Select",
                                                   new[] { sourceType, destinationType },
                                                   property, BuildNew(sourceType, destinationType)));
        }


        public Expression<Func<TSource, TDestination, TDestination>> BuildExisting<TSource, TDestination>()
        {
            return (Expression<Func<TSource, TDestination, TDestination>>) BuildExisting(typeof (TSource), typeof (TDestination));
        }

        public LambdaExpression BuildExisting(Type sourceType, Type destinationType)
        {
            return existingObjectMapperBuilder.Build(GetTypeMap(Tuple.Create(sourceType, destinationType)));
        }

        public void AddMapping(TypeMap typeMap)
        {
            mappers.Add(Tuple.Create(typeMap.SourceType, typeMap.DestinationType), typeMap);
        }

        #endregion

        private TypeMap GetTypeMap(Tuple<Type, Type> tuple)
        {
            TypeMap typeMap;
            if (mappers.TryGetValue(tuple, out typeMap) == false)
                throw new KeyNotFoundException(tuple.ToString());
            return typeMap;
        }
    }
}