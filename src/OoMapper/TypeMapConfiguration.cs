using System;
using System.Collections.Generic;
using System.Linq;

namespace OoMapper
{
    public class TypeMapConfiguration : ITypePair
    {
        private readonly Type destinationType;
        private readonly ICollection<Tuple<Type, Type>> includes = new List<Tuple<Type, Type>>();
        private readonly IDictionary<string, PropertyMapConfiguration> propertyMapConfigurations = new Dictionary<string, PropertyMapConfiguration>();
        private readonly Type sourceType;

        public TypeMapConfiguration(Type sourceType, Type destinationType)
        {
            this.sourceType = sourceType;
            this.destinationType = destinationType;
        }

        public Type SourceType
        {
            get { return sourceType; }
        }

        public Type DestinationType
        {
            get { return destinationType; }
        }

        public IEnumerable<Tuple<Type, Type>> Includes
        {
            get { return includes; }
        }

        public void AddPropertyMapConfiguration(PropertyMapConfiguration pmc)
        {
            propertyMapConfigurations[pmc.DestinationMemberName] = pmc;
        }

        public PropertyMapConfiguration GetPropertyMapConfiguration(string destinationMemberName)
        {
            PropertyMapConfiguration pmc;
            propertyMapConfigurations.TryGetValue(destinationMemberName, out pmc);
            return pmc;
        }

        public void Include<TSource, TDestination>()
        {
            includes.Add(Tuple.Create(typeof (TSource), typeof (TDestination)));
        }

        public bool Including(TypeMapConfiguration other)
        {
            return includes.Contains(Tuple.Create(other.SourceType, other.DestinationType));
        }

        public bool HasIncludes()
        {
            return includes.Any();
        }
    }
}