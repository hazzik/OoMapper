namespace OoMapper
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal class EnumerableToEnumerableMapper : ISourceMemberResolver
    {
        public Expression BuildSource(Expression expression, Type destinationType, IMappingConfiguration mappingConfiguration)
        {
            if (!expression.Type.IsEnumerable() || !destinationType.IsEnumerable())
                return null;

            var isArray = destinationType.IsArray;
            var sourceElementType = TypeUtils.GetElementTypeOfEnumerable(expression.Type);
            var destinationElementType = TypeUtils.GetElementTypeOfEnumerable(destinationType);
            return Expression.Convert(CreateSelect(mappingConfiguration, sourceElementType, destinationElementType, expression, isArray ? "ToArray" : "ToList"), destinationType);
        }

        private static Expression CreateSelect(IMappingConfiguration mappingConfiguration, Type sourceType, Type destinationType, Expression property, string methodName)
        {
            return Expression.Call(typeof (Enumerable), methodName, new[] {destinationType},
                                   Expression.Call(typeof (Enumerable), "Select",
                                                   new[] {sourceType, destinationType},
                                                   property, mappingConfiguration.BuildNew(sourceType, destinationType)));
        }
    }
}
