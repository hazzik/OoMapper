using System.Linq;
using System.Linq.Expressions;

namespace OoMapper
{
    public class NewObjectMapperBuilder : IObjectMapperBuilder
    {
        public LambdaExpression Build(TypeMap typeMap, IMappingConfiguration configuration)
        {
            const string name = "src";
            ParameterExpression source = Expression.Parameter(typeMap.SourceType, name);
            MemberAssignment[] bindings = typeMap.PropertyMaps
                .Select(m => m.BuildBind(source, configuration))
                .ToArray();
            return Expression.Lambda(
                Expression.Condition(Expression.Equal(source, Expression.Constant(null)),
                                     Expression.Default(typeMap.DestinationType),
                                     Expression.MemberInit(Expression.New(typeMap.DestinationType), bindings)), source);
        }
    }
}