using System.Linq;
using System.Linq.Expressions;

namespace OoMapper
{
    public class ExistingObjectMapperBuilder : IObjectMapperBuilder
    {
        public LambdaExpression Build(TypeMap typeMap)
        {
            const string name = "src";
            ParameterExpression source = Expression.Parameter(typeMap.SourceType, name);
            ParameterExpression destination = Expression.Parameter(typeMap.DestinationType, "dst");
            Expression[] bindings = typeMap.PropertyMaps
                .Select(m => m.BuildAssign(destination, source))
                .Concat(new[] {destination})
                .ToArray();
            return Expression.Lambda(
                Expression.Block(bindings),
                source,
                destination);
        }
    }
}