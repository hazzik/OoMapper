using System;
using System.Linq.Expressions;

namespace OoMapper
{
    public interface IMappingConfiguration 
    {
        LambdaExpression BuildNew(Type sourceType, Type destinationType);
        LambdaExpression BuildExisting(Type sourceType, Type destinationType);
        Expression BuildNewExpressionBody(Expression expression, Type destinationType);
    }
}