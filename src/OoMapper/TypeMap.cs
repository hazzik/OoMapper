using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OoMapper
{
    public class TypeMap : ITypePair
	{
	    private readonly ICollection<PropertyMap> propertyMaps;

	    public TypeMap(Type sourceType, Type destinationType, ICollection<PropertyMap> propertyMaps)
		{
	        SourceType = sourceType;
			DestinationType = destinationType;

	        this.propertyMaps = propertyMaps;
		}

		public Type SourceType { get; private set; }

		public Type DestinationType { get; private set; }

	    public IEnumerable<PropertyMap> PropertyMaps
	    {
	        get { return propertyMaps; }
	    }

		public IEnumerable<Tuple<Type, Type>> Includes
		{
			get { return includes; }
		}

	    public PropertyMap GetPropertyMapFor(MemberInfo destinationMember)
		{
		    return PropertyMaps.FirstOrDefault(map => map.DestinationMember.Name.Equals(destinationMember.Name, StringComparison.Ordinal));
		}

		private readonly ICollection<Tuple<Type, Type>> includes = new List<Tuple<Type, Type>>();

		public void Include<TSource, TDestination>()
		{
			includes.Add(Tuple.Create(typeof (TSource), typeof (TDestination)));
		}
	}
}