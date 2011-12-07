using System;
using System.Collections.Generic;
using System.Linq;

namespace OoMapper
{
    class UserDefinedConfiguration : IUserDefinedConfiguration
    {
        private readonly ICollection<ITypeMapConfiguration> typeMapConfigurations = new List<ITypeMapConfiguration>();

        public void AddTypeMapConfiguration(ITypeMapConfiguration tmc)
        {
            typeMapConfigurations.Add(tmc);
        }

        public ITypeMapConfiguration FindTypeMapConfiguration(Type sourceType, Type destinationType)
        {
            var typeMap = typeMapConfigurations.FirstOrDefault(x => x.SourceType == sourceType && x.DestinationType == destinationType);
            if (typeMap != null)
                return typeMap;
            typeMap = sourceType.GetInterfaces()
                .Select(@interface => FindTypeMapConfiguration(@interface, destinationType))
                .FirstOrDefault(tm => tm != null);
            if (typeMap != null)
                return typeMap;
            Type baseType = sourceType.BaseType;
            if (baseType != null)
                return FindTypeMapConfiguration(baseType, destinationType);
            return null;
        }

        public IEnumerable<ITypeMapConfiguration> InheritedConfigurations(ITypeMapConfiguration tmc)
        {
            var selectMany = typeMapConfigurations.Where(x => x.Including(tmc))
                .SelectMany(InheritedConfigurations);

            yield return tmc;
            foreach (TypeMapConfiguration iitmc in selectMany)
                yield return iitmc;
        }
    }
}