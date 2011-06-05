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
            return Expression.Lambda(BuildNewExpressionBody(source, destinationType), source);
        }

        public Expression BuildNewExpressionBody(Expression expression, Type destinationType)
        {
            var sourceType = expression.Type;
            if (destinationType == sourceType || destinationType.IsAssignableFrom(sourceType))
            {
                return expression;
            }
            if (sourceType.IsDictionary() && destinationType.IsDictionary())
            {
                var sourceElementType = TypeUtils.GetElementTypeOfEnumerable(sourceType);
                var destinationElementType = TypeUtils.GetElementTypeOfEnumerable(destinationType);

                ParameterExpression parameter = Expression.Parameter(sourceElementType, "src");

                Type destinationKeyType = destinationElementType.GetProperty("Key").PropertyType;
                Type destinationValueType = destinationElementType.GetProperty("Value").PropertyType;

                MethodCallExpression call = Expression.Call(typeof (Enumerable), "ToDictionary", new[] {sourceElementType, destinationKeyType, destinationValueType},
                                                            expression,
                                                            CreateSelector(destinationKeyType, parameter, "Key"),
                                                            CreateSelector(destinationValueType, parameter, "Value"));
                return call;
            }
            if (sourceType.IsEnumerable() && destinationType.IsEnumerable())
            {
                bool isArray = destinationType.IsArray;
                Type sourceElementType = TypeUtils.GetElementTypeOfEnumerable(sourceType);
                Type destinationElementType = TypeUtils.GetElementTypeOfEnumerable(destinationType);
                return Expression.Convert(CreateSelect(sourceElementType, destinationElementType, expression, isArray ? "ToArray" : "ToList"), destinationType);
            }
            if (destinationType == typeof(string))
            {
                return Expression.Call(expression, "ToString", new Type[0]);
            }
            try
            {
                return Expression.Convert(expression, destinationType);
            }
            catch (InvalidOperationException)
            {
                LambdaExpression lambda = newObjectMapperBuilder.Build(GetTypeMap(sourceType, destinationType));
                return new ParameterRewriter(lambda.Parameters[0], expression).Visit(lambda.Body);
            }
        }

        private LambdaExpression CreateSelector(Type destinationType, ParameterExpression source, string sourcePropertyName)
        {
            return Expression.Lambda(BuildNewExpressionBody(Expression.Property(source, sourcePropertyName), destinationType), source);
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

        private Expression CreateSelect(Type sourceType, Type destinationType, Expression property, string methodName)
        {
            return Expression.Call(typeof (Enumerable), methodName, new[] {destinationType},
                                   Expression.Call(typeof (Enumerable), "Select",
                                                   new[] {sourceType, destinationType},
                                                   property, BuildNew(sourceType, destinationType)));
        }

        private TypeMap GetTypeMap(Type sourceType, Type destinationType)
        {
            TypeMap map = FindTypeMap(sourceType, destinationType);
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
            Type baseType = sourceType.BaseType;
            if (baseType != null)
                return FindTypeMap(baseType, destinationType);
            return null;
        }
    }
}