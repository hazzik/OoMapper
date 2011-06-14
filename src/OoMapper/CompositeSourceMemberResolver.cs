using System;
using System.Linq.Expressions;

namespace OoMapper
{
    public class CompositeSourceMemberResolver : ISourceMemberResolver
    {
        private readonly ISourceMemberResolver[] resolvers;

        public CompositeSourceMemberResolver(params ISourceMemberResolver[] resolvers)
        {
            this.resolvers = resolvers;
        }

        public Expression BuildSource(Expression x, Type destinationType, IMappingConfiguration mappingConfiguration, IMappingOptions options)
        {
            Expression expression = x;
            Expression condition = Expression.Constant(false);
            foreach (var resolver in resolvers)
            {
                if (options.SupportNullHandling && expression.Type.IsValueType == false)
                    condition = Expression.OrElse(condition, Expression.Equal(expression, Expression.Constant(null)));
                expression = resolver.BuildSource(expression, destinationType, mappingConfiguration, options);
            }
            return Expression.Condition(condition, Expression.Default(expression.Type), expression, expression.Type);
        }
    }
}