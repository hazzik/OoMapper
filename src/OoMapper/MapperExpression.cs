using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    public class MapperExpression<TSource, TDestination>
    {
        private readonly TypeMapConfiguration tmc;
        private readonly IMappingConfiguration configuration;

        public MapperExpression(TypeMapConfiguration tmc, IMappingConfiguration configuration)
        {
            this.tmc = tmc;
            this.configuration = configuration;
        }

        public MapperExpression<TSource, TDestination> ForMember<TProperty>(Expression<Func<TDestination, TProperty>> member, Action<PropertyMapExpression<TSource>> options)
        {
            MemberInfo mi = GetMemberInfo(member);
            var name = mi.Name;
            var pmc = new PropertyMapConfiguration(name);
            options(new PropertyMapExpression<TSource>(pmc, configuration));
            tmc.AddPropertyMapConfiguration(pmc);
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
		    tmc.Include<TSourceChild, TDestinationChild>();
			return this;
		}
    }
}