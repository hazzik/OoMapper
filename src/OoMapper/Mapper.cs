using System;

namespace OoMapper
{
    public static class Mapper
    {
        private static readonly IMappingConfiguration configuration = new MappingConfiguration();

        public static void Reset()
        {
            configuration.Reset();
        }

        public static void CreateMap<TSource, TDestination>()
        {
            Type sourceType = typeof (TSource);
            Type destinationType = typeof (TDestination);

            var typeMap = new TypeMap(sourceType, destinationType, configuration);

            Tuple<Type, Type> key = Tuple.Create(sourceType, destinationType);

            configuration.AddMapping(typeMap, key);
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
}