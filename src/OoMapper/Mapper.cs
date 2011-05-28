using System;

namespace OoMapper
{
    public static class Mapper
    {
        private static IMappingConfiguration configuration = new MappingConfiguration();

        public static void Reset()
        {
            configuration = new MappingConfiguration();
        }

        public static MapperExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            Type sourceType = typeof (TSource);
            Type destinationType = typeof (TDestination);

            var typeMap = new TypeMap(sourceType, destinationType, configuration);

            configuration.AddMapping(typeMap);

            return new MapperExpression<TSource, TDestination>(typeMap, configuration);
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            return configuration.BuildNew<TSource, TDestination>().Compile().Invoke(source);
        }

        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return configuration.BuildExisting<TSource, TDestination>().Compile().Invoke(source, destination);
        }

        public static object Map(object source, Type sourceType, Type destinationType)
        {
            return configuration.BuildNew(sourceType, destinationType).Compile().DynamicInvoke(source);
        }

        public static object Map(object source, object destination, Type sourceType, Type destinationType)
        {
            return configuration.BuildExisting(sourceType, destinationType).Compile().DynamicInvoke(source, destination);
        }
    }
}