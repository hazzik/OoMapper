using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OoMapper
{
    public class MappingConfiguration : IMappingConfiguration
    {
        private readonly IObjectMapperBuilder existingObjectMapperBuilder = new CachedObjectMapperBuilder(new ExistingObjectMapperBuilder());
        private readonly Lazy<DynamicMapperBuilder> lazyDynamicMapperBuilder = new Lazy<DynamicMapperBuilder>(DynamicMapperBuilder.Create);

        private readonly ICache<Tuple<Type, Type>, TypeMap> mappers =
            new Cache<Tuple<Type, Type>, TypeMap>();

        private readonly IObjectMapperBuilder newObjectMapperBuilder = new CachedObjectMapperBuilder(new NewObjectMapperBuilder());
        private readonly ISet<ITypeMapConfiguration> processed = new HashSet<ITypeMapConfiguration>();
    	private readonly ISourceMemberResolver sourceMemberResolver = new ConvertSourceMemberResolver();
        private readonly IUserDefinedConfiguration userDefinedConfiguration = new UserDefinedConfiguration();

        private DynamicMapperBuilder DynamicMapperBuilder
        {
            get { return lazyDynamicMapperBuilder.Value; }
        }

        #region IMappingConfiguration Members

        public Expression BuildSource(Expression expression, Type destinationType, IMappingConfiguration cfg)
        {
            Type sourceType = expression.Type;
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
                Expression<Func<DynamicMapperBase>> mapper = () => GetDynamicMapper(map);
                return Expression.Convert(Expression.Call(mapper.Body, "DynamicMap", Type.EmptyTypes, expression), destinationType);
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

    	private readonly IDictionary<TypeMapConfiguration, DynamicMapperBase> instances = new Dictionary<TypeMapConfiguration, DynamicMapperBase>();

        private DynamicMapperBase GetDynamicMapper(TypeMapConfiguration map)
    	{
    		DynamicMapperBase res;
    		if(instances.TryGetValue(map, out res))
    			return res;
			
    		TypeMap[] typeMaps = GetTypeMapsWithIncludes(map).ToArray();
    		Type dynamicMapper = DynamicMapperBuilder.BuildDynamicMapperType(typeMaps);
    		var dynamicMapperBase = (DynamicMapperBase)Activator.CreateInstance(dynamicMapper, this);
    		instances.Add(map, dynamicMapperBase);
    		return dynamicMapperBase;
    	}

    	public LambdaExpression BuildNew(Type sourceType, Type destinationType)
        {
            ParameterExpression source = Expression.Parameter(sourceType, "src");
            return Expression.Lambda(sourceMemberResolver.BuildSource(source, destinationType, this), source);
        }

        public LambdaExpression BuildExisting(Type sourceType, Type destinationType)
        {
            return existingObjectMapperBuilder.Build(GetTypeMap(sourceType, destinationType), this);
        }

        #endregion

        private IEnumerable<TypeMap> GetTypeMapsWithIncludes(ITypeMapConfiguration map)
        {
        	yield return CreateOrGetTypeMap(map);
			foreach (var include in map.Includes.SelectMany(x => GetTypeMapsWithIncludes(GetTypeMapConfiguration(x))))
				yield return include;
        }

    	private TypeMapConfiguration GetTypeMapConfiguration(Tuple<Type, Type> include)
    	{
    		Type sourceType = include.Item1;
    		Type destinationType = include.Item2;
    		TypeMapConfiguration map = userDefinedConfiguration.FindTypeMapConfiguration(sourceType, destinationType);
    		if (map == null) throw new KeyNotFoundException(include.ToString());
    		return map;
    	}

    	private LambdaExpression CreateSelector(Type destinationType, ParameterExpression source, string sourcePropertyName)
        {
            MemberExpression property = Expression.Property(source, sourcePropertyName);
            return Expression.Lambda(sourceMemberResolver.BuildSource(property, destinationType, this), source);
        }

        private Expression CreateSelect(Type sourceType, Type destinationType, Expression property, string methodName)
        {
            return Expression.Call(typeof (Enumerable), methodName, new[] {destinationType},
                                   Expression.Call(typeof (Enumerable), "Select",
                                                   new[] {sourceType, destinationType},
                                                   property, BuildNew(sourceType, destinationType)));
        }

        private TypeMap CreateOrGetTypeMap(ITypeMapConfiguration map)
        {
            return mappers.GetOrAdd(Tuple.Create(map.SourceType, map.DestinationType), k => TypeMapBuilder.CreateTypeMap(map, userDefinedConfiguration));
        }

        private TypeMap GetTypeMap(Type sourceType, Type destinationType)
        {
            var map = userDefinedConfiguration.FindTypeMapConfiguration(sourceType, destinationType);
            if (map == null) throw new KeyNotFoundException(Tuple.Create(sourceType, destinationType).ToString());
            return CreateOrGetTypeMap(map);
        }

        public void AddTypeMapConfiguration(ITypeMapConfiguration tmc)
        {
            userDefinedConfiguration.AddTypeMapConfiguration(tmc);
        }
    }
}