namespace OoMapper
{
	using System;
	using System.Collections.Generic;

	public class DynamicMapperBase
	{
		private readonly IDictionary<object, object> instances = new Dictionary<object, object>();

		protected static Func<TSource, TDestination, TDestination> Compile<TSource, TDestination>(IMappingConfiguration configuration)
		{
			return (Func<TSource, TDestination, TDestination>) configuration.BuildExisting(typeof (TSource), typeof (TDestination)).Compile();
		}

		public object DynamicMap(object x)
		{
			return ((dynamic) this).Map((dynamic) x);
		}

		protected TDestination TryMap<TSource, TDestination>(TSource source, Func<TSource, TDestination, TDestination> map) where TSource : class
		{
			Type sourceType = typeof (TSource);
			if (sourceType.IsValueType == false && source == default(TSource))
				return default(TDestination);

			object res;
			if (instances.TryGetValue(source, out res))
				return (TDestination) res;

			var destination = (TDestination) Activator.CreateInstance(typeof (TDestination));
			instances.Add(source, destination);
			map(source, destination);
			return destination;
		}
	}
}
