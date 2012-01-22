namespace OoMapper
{
    using System;
    using System.Linq.Expressions;

    internal class ObjectToStringMapper : ISourceMemberResolver
    {
        #region ISourceMemberResolver Members

        public Expression BuildSource(Expression expression, Type destinationType, IMappingConfiguration mappingConfiguration)
        {
            if (destinationType != typeof (string))
                return null;

            return Expression.Call(expression, "ToString", Type.EmptyTypes);
        }

        #endregion
    }
}
