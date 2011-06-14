using System;
using System.Collections.Generic;

namespace OoMapper
{
    public interface IUserDefinedConfiguration
    {
        void AddTypeMapConfiguration(TypeMapConfiguration tmc);
        TypeMapConfiguration FindTypeMapConfiguration(Type sourceType, Type destinationType);
        IEnumerable<TypeMapConfiguration> InheritedConfigurations(TypeMapConfiguration tmc);
    }
}