using System;
using System.Collections.Generic;
using System.Linq;

namespace OoMapper
{
    public class TypeMap : ITypePair
    {
        private readonly ICollection<PropertyMap> propertyMaps = new List<PropertyMap>();

	    public TypeMap(Type sourceType, Type destinationType, IEnumerable<PropertyMap> propertyMaps)
		{
	        SourceType = sourceType;
			DestinationType = destinationType;

	        this.propertyMaps = propertyMaps.ToList();
		}

		public Type SourceType { get; private set; }

		public Type DestinationType { get; private set; }

	    public IEnumerable<PropertyMap> PropertyMaps
	    {
	        get { return propertyMaps; }
	    }
    }
}