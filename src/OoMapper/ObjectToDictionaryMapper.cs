using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    internal class ObjectToDictionaryMapper : ISourceMemberResolver
    {
        private readonly HashSet<string> excluded = new HashSet<string>(new[] {"GetType", "ToString", "GetHashCode"});

        public Expression BuildSource(Expression expression, Type destinationType, IMappingConfiguration mappingConfiguration)
        {
            if (!destinationType.IsDictionary())
            {
                return null;
            }

            var destinationElementType = TypeUtils.GetElementTypeOfEnumerable(destinationType);

            var add = destinationType.GetMethod("Add");
            var destinationKeyType = destinationElementType.GetProperty("Key").PropertyType;
            var destinationValueType = destinationElementType.GetProperty("Value").PropertyType;

            IEnumerable<ElementInit> initializers = expression.Type.GetReadableMembers()
                .Where(m => !excluded.Contains(m.Name))
                .Where(m => !IsPropertyGetter(m))
                .Select(property => Expression.ElementInit(add,
                    BuildSource(mappingConfiguration, destinationKeyType, Expression.Constant(property.Name)),
                    BuildSource(mappingConfiguration, destinationValueType, ExpressionEx.Member(expression, property))));

            return Expression.ListInit(Expression.New(destinationType), initializers);
        }

        private static bool IsPropertyGetter(MemberInfo member)
        {
            var method = member as MethodInfo;
            if (method != null)
            {
                return method.IsHideBySig && method.IsSpecialName && member.Name.StartsWith("get_");
            }
            return false;
        }

        private static Expression BuildSource(IMappingConfiguration mappingConfiguration, Type destinationValueType, Expression member)
        {
            return mappingConfiguration.BuildSource(member, destinationValueType, mappingConfiguration);
        }
    }
}