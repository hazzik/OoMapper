using System;
using System.Linq.Expressions;

namespace OoMapper
{
    public interface IMappingConfiguration : ISourceMemberResolver
    {
        LambdaExpression BuildNew(Type sourceType, Type destinationType, IMappingOptions options);
        LambdaExpression BuildExisting(Type sourceType, Type destinationType, IMappingOptions options);
    }
}