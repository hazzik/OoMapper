using System;
using System.Collections.Generic;
using System.Linq;

namespace OoMapper
{
    public interface ITypeMapConfiguration
    {
        Type SourceType { get; }
        Type DestinationType { get; }
        IEnumerable<Tuple<Type, Type>> Includes { get; }
        void AddPropertyMapConfiguration(IPropertyMapConfiguration pmc);
        IPropertyMapConfiguration GetPropertyMapConfiguration(string destinationMemberName);
        void Include<TSource, TDestination>();
        bool Including(ITypeMapConfiguration other);
        bool HasIncludes();
    }

    public class TypeMapConfiguration : ITypePair, ITypeMapConfiguration
    {
        private readonly Type destinationType;
        private readonly ICollection<Tuple<Type, Type>> includes = new HashSet<Tuple<Type, Type>>();
        private readonly IDictionary<string, IPropertyMapConfiguration> propertyMapConfigurations = new Dictionary<string, IPropertyMapConfiguration>();
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

        public void AddPropertyMapConfiguration(IPropertyMapConfiguration pmc)
        {
            propertyMapConfigurations[pmc.DestinationMemberName] = pmc;
        }

        public IPropertyMapConfiguration GetPropertyMapConfiguration(string destinationMemberName)
        {
            IPropertyMapConfiguration pmc;
            propertyMapConfigurations.TryGetValue(destinationMemberName, out pmc);
            return pmc;
        }

        public void Include<TSource, TDestination>()
        {
            includes.Add(Tuple.Create(typeof (TSource), typeof (TDestination)));
        }

        public bool Including(ITypeMapConfiguration other)
        {
            return includes.Contains(Tuple.Create(other.SourceType, other.DestinationType));
        }

        public bool HasIncludes()
        {
            return includes.Any();
        }

        public override string ToString()
        {
            return string.Format("{0} -> {1}", sourceType, destinationType);
        }
    }
}