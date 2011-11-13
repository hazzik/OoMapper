using System;
using System.Collections.Generic;

namespace OoMapper
{
    public interface IUserDefinedConfiguration
    {
        void AddTypeMapConfiguration(ITypeMapConfiguration tmc);
        ITypeMapConfiguration FindTypeMapConfiguration(Type sourceType, Type destinationType);
        IEnumerable<ITypeMapConfiguration> InheritedConfigurations(ITypeMapConfiguration tmc);
    }
}