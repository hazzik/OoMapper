using System;
using System.Linq.Expressions;

namespace OoMapper
{
    public class PropertyMapExpression<TSource>
    {
        private readonly PropertyMapConfiguration pmc;
        private readonly IMappingConfiguration configuration;

        public PropertyMapExpression(PropertyMapConfiguration pmc, IMappingConfiguration configuration)
        {
            this.pmc = pmc;
            this.configuration = configuration;
        }

        public void Ignore()
        {
            pmc.Ignore();
        }

        public void MapFrom<TProperty>(Expression<Func<TSource, TProperty>> sourceMember)
        {
            pmc.SetCustomResolver(new LambdaSourceMemberResolver(sourceMember, configuration));
        }
    }
}