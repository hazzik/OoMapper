using System;
using System.Linq;
using System.Linq.Expressions;

namespace OoMapper
{
    public interface IObjectMapper
    {
        bool IsMatch(MappingContext context);
        Expression BuildSource(Expression expression, MappingContext context);
    }

    class ObjectMapper : IObjectMapper
    {
        public bool IsMatch(MappingContext context)
        {
            return context.MappingConfiguration.userDefinedConfiguration.FindTypeMapConfiguration(context.SourceType, context.DestinationType) != null;
        }

        public Expression BuildSource(Expression expression, MappingContext context)
        {
            TypeMapConfiguration map = context.MappingConfiguration.userDefinedConfiguration.FindTypeMapConfiguration(context.SourceType, context.DestinationType);
            if (context.MappingConfiguration.processed.Add(map) == false || map.HasIncludes() == false)
            {
                TypeMap typeMap = context.MappingConfiguration.CreateOrGetTypeMap(map);
                LambdaExpression lambda = context.MappingConfiguration.newObjectMapperBuilder.Build(typeMap, context.MappingConfiguration);
                return new ParameterRewriter(lambda.Parameters[0], expression).Visit(lambda.Body);
            }
            TypeMap[] typeMaps = context.MappingConfiguration.GetTypeMapsWithIncludes(map).ToArray();
            Type dynamicMapper = context.MappingConfiguration.DynamicMapperBuilder.CreateDynamicMapper(typeMaps);
            var instance = Activator.CreateInstance(dynamicMapper, context.MappingConfiguration);
            return Expression.Convert(Expression.Call(Expression.Constant(instance), "DynamicMap", Type.EmptyTypes, expression), context.DestinationType);
        }
    }

    class ConvertObjectMapper : IObjectMapper
    {
        public bool IsMatch(MappingContext context)
        {
            try
            {
                Expression.Convert(Expression.Default(context.SourceType), context.DestinationType);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public Expression BuildSource(Expression expression, MappingContext context)
        {
            return Expression.Convert(expression, context.DestinationType);
        }
    }

    class ObjectToStringMapper : IObjectMapper
    {
        public bool IsMatch(MappingContext context)
        {
            return context.DestinationType == typeof (string);
        }

        public Expression BuildSource(Expression expression, MappingContext context)
        {
            return Expression.Call(expression, "ToString", Type.EmptyTypes);
        }
    }

    class EnumerableToEnumerableMapper : IObjectMapper
    {
        public bool IsMatch(MappingContext context)
        {
            return context.SourceType.IsEnumerable() && context.DestinationType.IsEnumerable();
        }

        public Expression BuildSource(Expression expression, MappingContext context)
        {
            bool isArray = context.DestinationType.IsArray;
            Type sourceElementType = TypeUtils.GetElementTypeOfEnumerable(context.SourceType);
            Type destinationElementType = TypeUtils.GetElementTypeOfEnumerable(context.DestinationType);
            return Expression.Convert(context.MappingConfiguration.CreateSelect(sourceElementType, destinationElementType, expression, isArray ? "ToArray" : "ToList"), context.DestinationType);
        }
    }
}