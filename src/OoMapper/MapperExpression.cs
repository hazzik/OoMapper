using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    public class MapperExpression<TSource, TDestination>
    {
        private readonly TypeMapConfiguration tmc;

        public MapperExpression(TypeMapConfiguration tmc)
        {
            this.tmc = tmc;
        }

        public MapperExpression<TSource, TDestination> ForMember<TProperty>(Expression<Func<TDestination, TProperty>> member, Action<PropertyMapExpression<TSource>> options)
        {
            MemberInfo mi = GetMemberInfo(member);
            var name = mi.Name;
            var pmc = new PropertyMapConfiguration(name);
            options(new PropertyMapExpression<TSource>(pmc));
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

        public MapperExpression<TSource, TDestination> ForAllMembers(Action<PropertyMapExpression<TSource>> options)
        {
            var pmc = new PropertyMapConfiguration("*");
            options(new PropertyMapExpression<TSource>(pmc));
            tmc.AddPropertyMapConfiguration(pmc);
            return this;
        }
    }
}