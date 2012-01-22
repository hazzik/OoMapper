namespace OoMapper
{
    using System;
    using System.Linq.Expressions;

    internal class SameTypeMapper : ISourceMemberResolver
    {
        #region ISourceMemberResolver Members

        public Expression BuildSource(Expression expression, Type destinationType, IMappingConfiguration mappingConfiguration)
        {
            if (destinationType != expression.Type)
                return null;

            return expression;
        }

        #endregion
    }
}
