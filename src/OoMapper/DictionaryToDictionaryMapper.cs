namespace OoMapper
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal class DictionaryToDictionaryMapper : ISourceMemberResolver
    {
        public Expression BuildSource(Expression expression, Type destinationType, IMappingConfiguration mappingConfiguration)
        {
            if (!expression.Type.IsDictionary() || !destinationType.IsDictionary())
            {
                return null;
            }

            var sourceElementType = TypeUtils.GetElementTypeOfEnumerable(expression.Type);
            var destinationElementType = TypeUtils.GetElementTypeOfEnumerable(destinationType);

            var parameter = Expression.Parameter(sourceElementType, "src");

            var destinationKeyType = destinationElementType.GetProperty("Key").PropertyType;
            var destinationValueType = destinationElementType.GetProperty("Value").PropertyType;

            return Expression.Call(typeof(Enumerable), "ToDictionary", new[] { sourceElementType, destinationKeyType, destinationValueType },
                                   expression,
                                   CreateSelector(mappingConfiguration, destinationKeyType, parameter, "Key"),
                                   CreateSelector(mappingConfiguration, destinationValueType, parameter, "Value"));
        }

        private static LambdaExpression CreateSelector(IMappingConfiguration mappingConfiguration, Type destinationType, ParameterExpression source, string sourcePropertyName)
        {
            var property = Expression.Property(source, sourcePropertyName);
            return Expression.Lambda(mappingConfiguration.BuildSource(property, destinationType, mappingConfiguration), source);
        }
    }
}
