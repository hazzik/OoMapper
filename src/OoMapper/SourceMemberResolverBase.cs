using System;
using System.Linq.Expressions;

namespace OoMapper
{
    public abstract class SourceMemberResolverBase : ISourceMemberResolver
    {
        public virtual Expression BuildSource(Expression x, Type destinationType, IMappingConfiguration mappingConfiguration)
        {
            Expression expression = BuildSourceCore(x);
            return mappingConfiguration.BuildNewExpressionBody(expression, destinationType);
        }

        protected abstract Expression BuildSourceCore(Expression x);
    }
}