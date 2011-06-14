using System.Linq;
using System.Linq.Expressions;

namespace OoMapper
{
    public class NewObjectMapperBuilder : IObjectMapperBuilder
    {
        public LambdaExpression Build(TypeMap typeMap, IMappingConfiguration configuration, IMappingOptions options)
        {
            const string name = "src";
            ParameterExpression source = Expression.Parameter(typeMap.SourceType, name);
            MemberAssignment[] bindings = typeMap.PropertyMaps
                .Select(m => m.BuildBind(source, configuration, options))
                .ToArray();
            
            Expression memberInit = Expression.MemberInit(Expression.New(typeMap.DestinationType), bindings);

            Expression body = options.SupportNullHandling
                                  ? Expression.Condition(Expression.Equal(source, Expression.Constant(null)),
                                                         Expression.Default(typeMap.DestinationType),
                                                         memberInit)
                                  : memberInit;

            return Expression.Lambda(body, source);
        }
    }
}