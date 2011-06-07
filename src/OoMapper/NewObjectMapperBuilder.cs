using System.Linq;
using System.Linq.Expressions;

namespace OoMapper
{
    public class NewObjectMapperBuilder : IObjectMapperBuilder
    {
        public LambdaExpression Build(TypeMap typeMap)
        {
            const string name = "src";
            ParameterExpression source = Expression.Parameter(typeMap.SourceType, name);
            MemberAssignment[] bindings = typeMap.PropertyMaps
                .Select(m => m.BuildBind(source))
                .ToArray();
            return Expression.Lambda(
                Expression.MemberInit(
                    Expression.New(typeMap.DestinationType), bindings), source);
        }
    }
}