namespace OoMapper
{
    using System;
    using System.Linq.Expressions;

    internal class ObjectToStringMapper : ISourceMemberResolver
    {
        public Expression BuildSource(Expression expression, Type destinationType, IMappingConfiguration mappingConfiguration)
        {
            if (destinationType != typeof (string))
                return null;

            return Expression.Call(expression, "ToString", Type.EmptyTypes);
        }
    }
}
