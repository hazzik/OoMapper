namespace OoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public interface ITypeMapConfiguration
    {
        Type SourceType { get; }
        Type DestinationType { get; }
        IEnumerable<Tuple<Type, Type>> Includes { get; }
        void AddPropertyMapConfiguration(IPropertyMapConfiguration pmc);
        bool GetPropertyMapConfiguration(MemberInfo destination, out IPropertyMapConfiguration pmc);
        bool HasIncludes();
        void Include<TSource, TDestination>();
        bool Including(ITypeMapConfiguration other);
    }
}