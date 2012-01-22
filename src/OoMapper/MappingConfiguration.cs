namespace OoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class MappingConfiguration : IMappingConfiguration
    {
        private readonly ICache<Tuple<Type, Type>, Delegate> compilledExisting = new Cache<Tuple<Type, Type>, Delegate>();
        private readonly ICache<Tuple<Type, Type>, Delegate> compilledNew = new Cache<Tuple<Type, Type>, Delegate>();
       
        private readonly IObjectMapperBuilder existingObjectMapperBuilder = new CachedObjectMapperBuilder(new ExistingObjectMapperBuilder());
        private readonly Lazy<DynamicMapperBuilder> lazyDynamicMapperBuilder = new Lazy<DynamicMapperBuilder>(DynamicMapperBuilder.Create);
        private readonly ICache<Tuple<Type, Type>, TypeMap> mappers = new Cache<Tuple<Type, Type>, TypeMap>();
        private readonly IObjectMapperBuilder newObjectMapperBuilder = new CachedObjectMapperBuilder(new NewObjectMapperBuilder());
        private readonly ISet<ITypeMapConfiguration> processed = new HashSet<ITypeMapConfiguration>();
        private readonly ISourceMemberResolver sourceMemberResolver = new ConvertSourceMemberResolver();
        private readonly IUserDefinedConfiguration userDefinedConfiguration = new UserDefinedConfiguration();

        private DynamicMapperBuilder DynamicMapperBuilder
        {
            get { return lazyDynamicMapperBuilder.Value; }
        }

        public Expression BuildSource(Expression expression, Type destinationType, IMappingConfiguration cfg)
        {
            var sourceType = expression.Type;
            if (destinationType == sourceType)
            {
                return expression;
            }
            if (destinationType.IsAssignableFrom(sourceType))
            {
                return Expression.Convert(expression, destinationType);
            }
            var map = userDefinedConfiguration.FindTypeMapConfiguration(sourceType, destinationType);
            if (map != null)
            {
                if (processed.Add(map) == false || map.HasIncludes() == false)
                {
                    var typeMap = CreateOrGetTypeMap(map);
                    var lambda = newObjectMapperBuilder.Build(typeMap, this);
                    return new ParameterRewriter(lambda.Parameters[0], expression).Visit(lambda.Body);
                }
                var typeMaps = GetTypeMapsWithIncludes(map).ToArray();
                var dynamicMapper = DynamicMapperBuilder.BuildDynamicMapperType(typeMaps);
                var instance = Activator.CreateInstance(dynamicMapper, this);
                return Expression.Convert(Expression.Call(Expression.Constant(instance), "DynamicMap", Type.EmptyTypes, expression), destinationType);
            }
            if (sourceType.IsDictionary() && destinationType.IsDictionary())
            {
                var sourceElementType = TypeUtils.GetElementTypeOfEnumerable(sourceType);
                var destinationElementType = TypeUtils.GetElementTypeOfEnumerable(destinationType);

                var parameter = Expression.Parameter(sourceElementType, "src");

                var destinationKeyType = destinationElementType.GetProperty("Key").PropertyType;
                var destinationValueType = destinationElementType.GetProperty("Value").PropertyType;

                var call = Expression.Call(typeof (Enumerable), "ToDictionary", new[] {sourceElementType, destinationKeyType, destinationValueType},
                                           expression,
                                           CreateSelector(destinationKeyType, parameter, "Key"),
                                           CreateSelector(destinationValueType, parameter, "Value"));
                return call;
            }
            if (sourceType.IsEnumerable() && destinationType.IsEnumerable())
            {
                var isArray = destinationType.IsArray;
                var sourceElementType = TypeUtils.GetElementTypeOfEnumerable(sourceType);
                var destinationElementType = TypeUtils.GetElementTypeOfEnumerable(destinationType);
                return Expression.Convert(CreateSelect(sourceElementType, destinationElementType, expression, isArray ? "ToArray" : "ToList"), destinationType);
            }
            if (destinationType == typeof (string))
            {
                return Expression.Call(expression, "ToString", Type.EmptyTypes);
            }
            try
            {
                return Expression.Convert(expression, destinationType);
            }
            catch (InvalidOperationException)
            {
            }
            throw new KeyNotFoundException(Tuple.Create(sourceType, destinationType).ToString());
        }

        public LambdaExpression BuildNew(Type sourceType, Type destinationType)
        {
            var source = Expression.Parameter(sourceType, "src");
            return Expression.Lambda(sourceMemberResolver.BuildSource(source, destinationType, this), source);
        }

        public LambdaExpression BuildExisting(Type sourceType, Type destinationType)
        {
            return existingObjectMapperBuilder.Build(GetTypeMap(sourceType, destinationType), this);
        }

        public void AddTypeMapConfiguration(ITypeMapConfiguration tmc)
        {
            userDefinedConfiguration.AddTypeMapConfiguration(tmc);
        }

        public Delegate GetCompiledExisting(Type sourceType, Type destinationType)
        {
            return compilledExisting.GetOrAdd(Tuple.Create(sourceType, destinationType),
                                              key => BuildExisting(sourceType, destinationType).Compile());
        }

        public Delegate GetCompiledNew(Type sourceType, Type destinationType)
        {
            return compilledNew.GetOrAdd(Tuple.Create(sourceType, destinationType),
                                         key => BuildNew(sourceType, destinationType).Compile());
        }

        private TypeMap CreateOrGetTypeMap(ITypeMapConfiguration map)
        {
            return mappers.GetOrAdd(Tuple.Create(map.SourceType, map.DestinationType), k => TypeMapBuilder.CreateTypeMap(map, userDefinedConfiguration));
        }

        private Expression CreateSelect(Type sourceType, Type destinationType, Expression property, string methodName)
        {
            return Expression.Call(typeof (Enumerable), methodName, new[] {destinationType},
                                   Expression.Call(typeof (Enumerable), "Select",
                                                   new[] {sourceType, destinationType},
                                                   property, BuildNew(sourceType, destinationType)));
        }

        private LambdaExpression CreateSelector(Type destinationType, ParameterExpression source, string sourcePropertyName)
        {
            var property = Expression.Property(source, sourcePropertyName);
            return Expression.Lambda(sourceMemberResolver.BuildSource(property, destinationType, this), source);
        }

        private TypeMap GetTypeMap(Type sourceType, Type destinationType)
        {
            var map = userDefinedConfiguration.FindTypeMapConfiguration(sourceType, destinationType);
            if (map == null) throw new KeyNotFoundException(Tuple.Create(sourceType, destinationType).ToString());
            return CreateOrGetTypeMap(map);
        }

        private IEnumerable<TypeMap> GetTypeMapsWithIncludes(ITypeMapConfiguration map)
        {
            yield return CreateOrGetTypeMap(map);
            foreach (var include in map.Includes)
                yield return GetTypeMap(include.Item1, include.Item2);
        }
    }
}