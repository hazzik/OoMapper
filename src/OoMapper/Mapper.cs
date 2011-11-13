using System;

namespace OoMapper
{
    public static class Mapper
    {
        private static MappingConfiguration configuration = new MappingConfiguration();

        public static void Reset()
        {
            configuration = new MappingConfiguration();
        }

        public static MapperExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            Type sourceType = typeof (TSource);
            Type destinationType = typeof (TDestination);

            var tmc = new TypeMapConfiguration(sourceType, destinationType);

            configuration.AddTypeMapConfiguration(tmc);

            return new MapperExpression<TSource, TDestination>(tmc);
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            Type sourceType = typeof (TSource);
            Type destinationType = typeof (TDestination);
        	var expression = configuration.BuildNew(sourceType, destinationType);
        	var func = (Func<TSource, TDestination>)expression.Compile();
            return func.Invoke(source);
        }

        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);
            var expression = configuration.BuildExisting(sourceType, destinationType);
            var func = (Func<TSource, TDestination, TDestination>)expression.Compile();
            return func.Invoke(source, destination);
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