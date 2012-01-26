namespace OoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class TypeMapConfiguration : ITypePair, ITypeMapConfiguration
    {
        private readonly Type destinationType;
        private readonly ICollection<Tuple<Type, Type>> includes = new HashSet<Tuple<Type, Type>>();
        private readonly ICollection<IPropertyMapConfiguration> propertyMapConfigurations = new List<IPropertyMapConfiguration>();
        private readonly Type sourceType;

        public TypeMapConfiguration(Type sourceType, Type destinationType)
        {
            this.sourceType = sourceType;
            this.destinationType = destinationType;
        }

        public IEnumerable<Tuple<Type, Type>> Includes
        {
            get { return includes; }
        }

        public void AddPropertyMapConfiguration(IPropertyMapConfiguration pmc)
        {
            propertyMapConfigurations.Add(pmc);
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

        public bool GetPropertyMapConfiguration(MemberInfo destination, out IPropertyMapConfiguration pmc)
        {
            pmc = propertyMapConfigurations.OrderBy(x => x.Order)
                .FirstOrDefault(x => x.IsApplicableTo(destination));

            return pmc != null && pmc.IsMapped();
        }

        public Type SourceType
        {
            get { return sourceType; }
        }

        public Type DestinationType
        {
            get { return destinationType; }
        }

        public override string ToString()
        {
            return string.Format("{0} -> {1}", sourceType, destinationType);
        }
    }
}