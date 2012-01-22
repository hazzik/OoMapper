namespace OoMapper
{
    using System;
    using System.Linq.Expressions;

    internal class ConvertMapper : ISourceMemberResolver
    {
        #region ISourceMemberResolver Members

        public Expression BuildSource(Expression expression, Type destinationType, IMappingConfiguration mappingConfiguration)
        {
            try
            {
                return Expression.Convert(expression, destinationType);
            }
            catch (InvalidOperationException)
            {
            }
            return null;
        }

        #endregion
    }
}
