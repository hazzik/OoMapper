using System.Linq.Expressions;

namespace OoMapper
{
    public interface IObjectMapperBuilder
    {
        LambdaExpression Build(TypeMap typeMap, IMappingConfiguration configuration, IMappingOptions options);
    }
}