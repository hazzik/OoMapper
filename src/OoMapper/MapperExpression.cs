namespace OoMapper
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public class MapperExpression<TSource, TDestination>
    {
        private readonly ITypeMapConfiguration tmc;

        public MapperExpression(ITypeMapConfiguration tmc)
        {
            this.tmc = tmc;
        }

        public MapperExpression<TSource, TDestination> ForAllMembers(Action<PropertyMapExpression<TSource>> options)
        {
            return ForMembers(x => true, options);
        }

        public MapperExpression<TSource, TDestination> ForMember<TProperty>(Expression<Func<TDestination, TProperty>> member, Action<PropertyMapExpression<TSource>> options)
        {
            var mi = GetMemberInfo(member);
            return ForMembers(m => m.Name == mi.Name, options);
        }

        public MapperExpression<TSource, TDestination> ForMembers(Func<MemberInfo, bool> predicate, Action<PropertyMapExpression<TSource>> options)
        {
            var pmc = new PropertyMapConfiguration(predicate, 2000);
            options(new PropertyMapExpression<TSource>(pmc));
            tmc.AddPropertyMapConfiguration(pmc);
            return this;
        }

        public MapperExpression<TSource, TDestination> Include<TSourceChild, TDestinationChild>()
            where TSourceChild : TSource
            where TDestinationChild : TDestination
        {
            tmc.Include<TSourceChild, TDestinationChild>();
            return this;
        }

        private static MemberInfo GetMemberInfo<TProperty>(Expression<Func<TDestination, TProperty>> member)
        {
            var memberExpression = member.Body as MemberExpression;
            return memberExpression != null
                       ? memberExpression.Member
                       : null;
        }
    }
}