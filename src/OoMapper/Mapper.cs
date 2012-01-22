namespace OoMapper
{
    using System;

    public static class Mapper
    {
        private static MappingConfiguration configuration = new MappingConfiguration();

        public static MapperExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            var sourceType = typeof (TSource);
            var destinationType = typeof (TDestination);

            var tmc = new TypeMapConfiguration(sourceType, destinationType);

            configuration.AddTypeMapConfiguration(tmc);

            return new MapperExpression<TSource, TDestination>(tmc);
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            var mapper = (Func<TSource, TDestination>) configuration.GetCompiledNew(typeof (TSource), typeof (TDestination));
            return mapper.Invoke(source);
        }

        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            var mapper = (Func<TSource, TDestination, TDestination>) configuration.GetCompiledExisting(typeof (TSource), typeof (TDestination));
            return mapper.Invoke(source, destination);
        }

        public static object Map(object source, Type sourceType, Type destinationType)
        {
            var mapper = configuration.GetCompiledNew(sourceType, destinationType);
            return mapper.DynamicInvoke(source);
        }

        public static object Map(object source, object destination, Type sourceType, Type destinationType)
        {
            var mapper = configuration.GetCompiledExisting(sourceType, destinationType);
            return mapper.DynamicInvoke(source, destination);
        }

        public static void Reset()
        {
            configuration = new MappingConfiguration();
        }
    }
}