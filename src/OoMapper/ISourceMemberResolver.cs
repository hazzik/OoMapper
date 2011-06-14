using System;
using System.Linq.Expressions;

namespace OoMapper
{
    public interface ISourceMemberResolver
    {
        Expression BuildSource(Expression x, Type destinationType, IMappingConfiguration mappingConfiguration, IMappingOptions options);
    }
}