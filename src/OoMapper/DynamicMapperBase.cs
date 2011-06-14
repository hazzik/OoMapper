namespace OoMapper
{
	using System;

	public class DynamicMapperBase
	{
		protected static Func<TSource, TDestination> Compile<TSource, TDestination>(IMappingConfiguration configuration, IMappingOptions options)
		{
			return (Func<TSource, TDestination>) configuration.BuildNew(typeof (TSource), typeof (TDestination), options).Compile();
		}

		public object DynamicMap(object x)
		{
			return ((dynamic) this).Map((dynamic) x);
		}
	}
}