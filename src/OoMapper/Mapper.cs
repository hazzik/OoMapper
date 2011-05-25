using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    public static class Mapper
    {
        private static readonly IMappingConfiguration configuration = new MappingConfiguration();

        public static void Reset()
        {
            configuration.Reset();
        }

        public static MapperExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            Type sourceType = typeof (TSource);
            Type destinationType = typeof (TDestination);

            var typeMap = new TypeMap(sourceType, destinationType, configuration);

            Tuple<Type, Type> key = Tuple.Create(sourceType, destinationType);

            configuration.AddMapping(typeMap, key);

            return new MapperExpression<TSource, TDestination>(typeMap);
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            return configuration.BuildNew<TSource, TDestination>().Compile().Invoke(source);
        }

        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return configuration.BuildExisting<TSource, TDestination>().Compile().Invoke(source, destination);
        }
    }

    public class MapperExpression<TSource, TDestination>
    {
        private readonly TypeMap typeMap;

        public MapperExpression(TypeMap typeMap)
        {
            this.typeMap = typeMap;
        }

        public MapperExpression<TSource, TDestination> ForMember<TProperty>(Expression<Func<TDestination, TProperty>> member, Action<PropertyMapExpression<TSource>> options)
        {
            MemberInfo mi = GetMemberInfo(member);
            var propertyMap = typeMap.GetPropertyMapFor(mi);
            options(new PropertyMapExpression<TSource>(propertyMap));
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
    }

    public class PropertyMapExpression<TSource>
    {
        private readonly PropertyMap propertyMap;

        public PropertyMapExpression(PropertyMap propertyMap)
        {
            this.propertyMap = propertyMap;
        }

        public void Ignore()
        {
            propertyMap.IsIgnored = true;
        }

        public void MapFrom<TProperty>(Expression<Func<TSource, TProperty>> sourceMember)
        {
            propertyMap.SourceMemberResolver = new LambdaSourceMemberResolver(sourceMember);
        }
    }
}