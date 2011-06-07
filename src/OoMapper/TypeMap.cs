using System;
using System.Collections.Generic;
using System.Linq;

namespace OoMapper
{
    public class TypeMap : ITypePair
    {
        private readonly IDictionary<string, PropertyMap> propertyMaps = new Dictionary<string, PropertyMap>(StringComparer.Ordinal);

	    public TypeMap(Type sourceType, Type destinationType, IEnumerable<PropertyMap> propertyMaps)
		{
	        SourceType = sourceType;
			DestinationType = destinationType;

	        this.propertyMaps = propertyMaps
	            .ToDictionary(x => x.DestinationMemberName, x => x);
		}

		public Type SourceType { get; private set; }

		public Type DestinationType { get; private set; }

	    public IEnumerable<PropertyMap> PropertyMaps
	    {
	        get { return propertyMaps.Values; }
	    }

		public IEnumerable<Tuple<Type, Type>> Includes
		{
			get { return includes; }
		}

	    public PropertyMap GetPropertyMapFor(string name)
	    {
	        PropertyMap pm;
	        propertyMaps.TryGetValue(name, out pm);
	        return pm;
	    }

        private readonly ICollection<Tuple<Type, Type>> includes = new List<Tuple<Type, Type>>();

		public void Include<TSource, TDestination>()
		{
			includes.Add(Tuple.Create(typeof (TSource), typeof (TDestination)));
		}
	}
}