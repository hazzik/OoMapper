using System;
using System.Linq;

namespace OoMapper
{
    public static partial class Mapper
    {
        private static MappingConfiguration configuration = new MappingConfiguration();

        private static readonly MappingOptions defaultOptions = new MappingOptions()
                                                                    {
                                                                        SupportNullHandling = true,
                                                                    };

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
            var lambdaExpression = configuration.BuildNew(sourceType, destinationType, defaultOptions);
        	var func = (Func<TSource, TDestination>)lambdaExpression.Compile();
            return func.Invoke(source);
        }

        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);
            var func = (Func<TSource, TDestination, TDestination>)configuration.BuildExisting(sourceType, destinationType, defaultOptions).Compile();
            return func.Invoke(source, destination);
        }

        public static object Map(object source, Type sourceType, Type destinationType)
        {
            return configuration.BuildNew(sourceType, destinationType, defaultOptions).Compile().DynamicInvoke(source);
        }

        public static object Map(object source, object destination, Type sourceType, Type destinationType)
        {
            return configuration.BuildExisting(sourceType, destinationType, defaultOptions).Compile().DynamicInvoke(source, destination);
        }

        public static ProjectToExpression<T> Project<T>(this IQueryable<T> queryable)
        {
            return new ProjectToExpression<T>(queryable);
        }

        public class ProjectToExpression<T>
        {
            private readonly IQueryable<T> queryable;

            public ProjectToExpression(IQueryable<T> queryable)
            {
                this.queryable = queryable;
            }

            public TResult[] To<TResult>()
            {
                Type sourceType = typeof (IQueryable<T>);
                Type destinationType = typeof (TResult[]);
                var lambdaExpression = configuration.BuildNew(sourceType, destinationType, new MappingOptions
                                                                                               {
                                                                                                   SupportNullHandling = false
                                                                                               });
                var func = (Func<IQueryable<T>, TResult[]>)lambdaExpression.Compile();
                return func.Invoke(queryable);
            }
        }
    }

public interface IMappingOptions
{
    bool SupportNullHandling { get; }
}

    class MappingOptions : IMappingOptions
    {
        public bool SupportNullHandling { get; set; }
    }
}