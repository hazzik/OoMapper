using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    public class MapperExpression<TSource, TDestination>
    {
        private readonly TypeMap typeMap;
        private readonly IMappingConfiguration configuration;

        public MapperExpression(TypeMap typeMap, IMappingConfiguration configuration)
        {
            this.typeMap = typeMap;
            this.configuration = configuration;
        }

        public MapperExpression<TSource, TDestination> ForMember<TProperty>(Expression<Func<TDestination, TProperty>> member, Action<PropertyMapExpression<TSource>> options)
        {
            MemberInfo mi = GetMemberInfo(member);
            var propertyMap = typeMap.GetPropertyMapFor(mi);
            options(new PropertyMapExpression<TSource>(propertyMap, configuration));
            return this;
        }

        private static MemberInfo GetMemberInfo<TProperty>(Expression<Func<TDestination, TProperty>> member)
        {
            Expression expression = member.Body;
            if(expression is MemberExpression)
            {
                return (expression as MemberExpression).Member;
            }
            return null;
        }

		public MapperExpression<TSource, TDestination> Include<TSourceChild, TDestinationChild>()
			where TSourceChild : TSource
			where TDestinationChild : TDestination
		{
			typeMap.Include<TSourceChild, TDestinationChild>();
			return this;
		}
    }
}