using System;
using System.Collections.Generic;
using System.Linq;

namespace OoMapper
{
    class UserDefinedConfiguration : IUserDefinedConfiguration
    {
        private readonly ICollection<TypeMapConfiguration> typeMapConfigurations = new List<TypeMapConfiguration>();

        public void AddTypeMapConfiguration(TypeMapConfiguration tmc)
        {
            typeMapConfigurations.Add(tmc);
        }

        public TypeMapConfiguration FindTypeMapConfiguration(Type sourceType, Type destinationType)
        {
            TypeMapConfiguration typeMap = typeMapConfigurations.FirstOrDefault(x => x.SourceType == sourceType && x.DestinationType == destinationType);
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

        public IEnumerable<TypeMapConfiguration> InheritedConfigurations(TypeMapConfiguration tmc)
        {
            IEnumerable<TypeMapConfiguration> selectMany = typeMapConfigurations.Where(x => x.Including(tmc))
                .SelectMany(InheritedConfigurations);

            yield return tmc;
            foreach (TypeMapConfiguration iitmc in selectMany)
                yield return iitmc;
        }
    }
}