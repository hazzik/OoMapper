namespace OoMapper
{
    using System;
    using System.Linq.Expressions;

    internal class ConvertibleTypeMapper : ISourceMemberResolver
    {
        #region ISourceMemberResolver Members

        public Expression BuildSource(Expression expression, Type destinationType, IMappingConfiguration mappingConfiguration)
        {
            if (!destinationType.IsAssignableFrom(expression.Type))
                return null;

            return Expression.Convert(expression, destinationType);
        }

        #endregion
    }
}
