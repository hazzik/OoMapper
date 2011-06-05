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

        public LambdaExpression BuildNew(Type sourceType, Type destinationType)
        {
            ParameterExpression source = Expression.Parameter(sourceType, "src");
            if (sourceType.IsEnumerable() && destinationType.IsEnumerable())
            {
                var isArray = destinationType.IsArray;
                Type sourceElementType = TypeUtils.GetElementTypeOfEnumerable(sourceType);
                Type destinationElementType = TypeUtils.GetElementTypeOfEnumerable(destinationType);
                return Expression.Lambda(Expression.Convert(CreateSelect(sourceElementType, destinationElementType, source, isArray ? "ToArray" : "ToList"), destinationType), source);
            }
            if (destinationType == typeof(string))
            {
                return Expression.Lambda(Expression.Call(source, "ToString", new Type[0]), source);
            }
            try
            {
                return Expression.Lambda(Expression.Convert(source, destinationType), source);
            }
            catch (InvalidOperationException)
            {
                return newObjectMapperBuilder.Build(GetTypeMap(sourceType, destinationType));
            }
        }

        private Expression CreateSelect(Type sourceType, Type destinationType, Expression property, string methodName)
        {
            return Expression.Call(typeof (Enumerable), methodName, new[] {destinationType},
                                   Expression.Call(typeof (Enumerable), "Select",
                                                   new[] {sourceType, destinationType},
                                                   property, BuildNew(sourceType, destinationType)));
        }


        public LambdaExpression BuildExisting(Type sourceType, Type destinationType)
        {
            return existingObjectMapperBuilder.Build(GetTypeMap(sourceType, destinationType));
        }

        public void AddMapping(TypeMap typeMap)
        {
            mappers.Add(Tuple.Create(typeMap.SourceType, typeMap.DestinationType), typeMap);
        }

        #endregion

        private TypeMap GetTypeMap(Type sourceType, Type destinationType)
        {
            var map = FindTypeMap(sourceType, destinationType);
            if (map == null) throw new KeyNotFoundException(Tuple.Create(sourceType, destinationType).ToString());
            return map;
        }

        private TypeMap FindTypeMap(Type sourceType, Type destinationType)
        {
            Tuple<Type, Type> tuple = Tuple.Create(sourceType, destinationType);
            TypeMap typeMap;
            if (mappers.TryGetValue(tuple, out typeMap))
                return typeMap;
            typeMap = sourceType.GetInterfaces()
                .Select(@interface => FindTypeMap(@interface, destinationType))
                .FirstOrDefault(tm => tm != null);
            if (typeMap != null)
                return typeMap;
            var baseType = sourceType.BaseType;
            if (baseType != null)
                return FindTypeMap(baseType, destinationType);
            return null;
        }
    }
}