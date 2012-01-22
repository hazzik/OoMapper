using System;
using System.Linq.Expressions;

namespace OoMapper
{
    internal class ConvertSourceMemberResolver : ISourceMemberResolver
    {
        public Expression BuildSource(Expression x, Type destinationType, IMappingConfiguration mappingConfiguration)
        {
            return mappingConfiguration.BuildSource(x, destinationType, mappingConfiguration);
        }
    }
}