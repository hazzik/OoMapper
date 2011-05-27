using System;
using System.Linq.Expressions;

namespace OoMapper
{
    public class PropertyMapExpression<TSource>
    {
        private readonly PropertyMap propertyMap;
        private IMappingConfiguration configuration;

        public PropertyMapExpression(PropertyMap propertyMap, IMappingConfiguration configuration)
        {
            this.propertyMap = propertyMap;
            this.configuration = configuration;
        }

        public void Ignore()
        {
            propertyMap.IsIgnored = true;
        }

        public void MapFrom<TProperty>(Expression<Func<TSource, TProperty>> sourceMember)
        {
            propertyMap.SourceMemberResolver = new LambdaSourceMemberResolver(sourceMember, configuration);
        }
    }
}