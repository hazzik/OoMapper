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
        private readonly Lazy<DynamicMapperBuilder> lazyDynamicMapperBuilder = new Lazy<DynamicMapperBuilder>(DynamicMapperBuilder.Create);

        private readonly ConcurrentDictionary<Tuple<Type, Type>, TypeMap> mappers =
            new ConcurrentDictionary<Tuple<Type, Type>, TypeMap>();

        private readonly IObjectMapperBuilder newObjectMapperBuilder = new CachedObjectMapperBuilder(new NewObjectMapperBuilder());
        private readonly ISet<TypeMapConfiguration> processed = new HashSet<TypeMapConfiguration>();
        private readonly IUserDefinedConfiguration userDefinedConfiguration = new UserDefinedConfiguration();

        private DynamicMapperBuilder DynamicMapperBuilder
        {
            get { return lazyDynamicMapperBuilder.Value; }
        }

        #region IMappingConfiguration Members

        public LambdaExpression BuildNew(Type sourceType, Type destinationType)
        {
            ParameterExpression source = Expression.Parameter(sourceType, "src");
            return Expression.Lambda(BuildNewExpressionBody(source, destinationType), source);
        }

        public Expression BuildNewExpressionBody(Expression expression, Type destinationType)
        {
            Type sourceType = expression.Type;
            if (destinationType == sourceType || destinationType.IsAssignableFrom(sourceType))
            {
                return expression;
            }
            if (sourceType.IsDictionary() && destinationType.IsDictionary())
            {
                Type sourceElementType = TypeUtils.GetElementTypeOfEnumerable(sourceType);
                Type destinationElementType = TypeUtils.GetElementTypeOfEnumerable(destinationType);

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
            if (destinationType == typeof (string))
            {
                return Expression.Call(expression, "ToString", new Type[0]);
            }
            try
            {
                return Expression.Convert(expression, destinationType);
            }
            catch (InvalidOperationException)
            {
                TypeMapConfiguration map = userDefinedConfiguration.FindTypeMapConfiguration(sourceType, destinationType);
                if (map == null) throw new KeyNotFoundException(Tuple.Create(sourceType, destinationType).ToString());

                if (processed.Add(map) == false || map.HasIncludes() == false)
                {
                    TypeMap typeMap = CreateOrGetTypeMap(map);
                    LambdaExpression lambda = newObjectMapperBuilder.Build(typeMap, this);
                    return new ParameterRewriter(lambda.Parameters[0], expression).Visit(lambda.Body);
                }
                TypeMap[] typeMaps = GetTypeMapsWithIncludes(map).ToArray();
                Type dynamicMapper = DynamicMapperBuilder.CreateDynamicMapper(typeMaps);
                var instance = (DynamicMapperBase) Activator.CreateInstance(dynamicMapper, this);
                return Expression.Convert(Expression.Call(Expression.Constant(instance), "DynamicMap", Type.EmptyTypes, expression), destinationType);
            }
        }

        public void AddTypeMapConfiguration(TypeMapConfiguration tmc)
        {
            userDefinedConfiguration.AddTypeMapConfiguration(tmc);            
        }

        public LambdaExpression BuildExisting(Type sourceType, Type destinationType)
        {
            return existingObjectMapperBuilder.Build(GetTypeMap(sourceType, destinationType), this);
        }

        #endregion

        private TypeMap CreateOrGetTypeMap(TypeMapConfiguration map)
        {
            return mappers.GetOrAdd(Tuple.Create(map.SourceType, map.DestinationType), k => TypeMapBuilder.CreateTypeMap(map, userDefinedConfiguration));
        }

        private IEnumerable<TypeMap> GetTypeMapsWithIncludes(TypeMapConfiguration map)
        {
            yield return CreateOrGetTypeMap(map);
            foreach (var include in map.Includes)
                yield return GetTypeMap(include.Item1, include.Item2);
        }

        private LambdaExpression CreateSelector(Type destinationType, ParameterExpression source, string sourcePropertyName)
        {
            return Expression.Lambda(BuildNewExpressionBody(Expression.Property(source, sourcePropertyName), destinationType), source);
        }

        private Expression CreateSelect(Type sourceType, Type destinationType, Expression property, string methodName)
        {
            return Expression.Call(typeof (Enumerable), methodName, new[] {destinationType},
                                   Expression.Call(typeof (Enumerable), "Select",
                                                   new[] {sourceType, destinationType},
                                                   property, BuildNew(sourceType, destinationType)));
        }

        private TypeMap GetTypeMap(Type sourceType, Type destinationType)
        {
            TypeMapConfiguration map = userDefinedConfiguration.FindTypeMapConfiguration(sourceType, destinationType);
            if (map == null) throw new KeyNotFoundException(Tuple.Create(sourceType, destinationType).ToString());
            return CreateOrGetTypeMap(map);
        }
    }
}