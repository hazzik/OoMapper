using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    public class TypeMap
    {
        private readonly IEnumerable<M> array;
        private readonly Type destinationType;
        private readonly Type sourceType;

        public TypeMap(Type sourceType, Type destinationType)
        {
            PropertyInfo[] sourceMembers = sourceType.GetProperties();
            PropertyInfo[] destinationMembers = destinationType.GetProperties();

            this.sourceType = sourceType;
            this.destinationType = destinationType;

            array = destinationMembers
                .Select(x => new M(x, FindMembers(x.Name, sourceMembers)))
                .ToArray();
        }

        private Type SourceType
        {
            get { return sourceType; }
        }

        public LambdaExpression BuildNew()
        {
            const string name = "src";

            ParameterExpression source = Expression.Parameter(SourceType, name);

            MemberAssignment[] bindings = array
                .Select(m => Expression.Bind(m.Destination, m.BuildSource(source)))
                .ToArray();

            return Expression.Lambda(
                Expression.MemberInit(
                    Expression.New(destinationType), bindings), source);
        }

        public LambdaExpression BuildExisting()
        {
            const string name = "src";

            ParameterExpression source = Expression.Parameter(SourceType, name);
            ParameterExpression destination = Expression.Parameter(destinationType, "dst");

            var bindings = array
                .Select(m => (Expression)Expression.Assign(Expression.MakeMemberAccess(destination, m.Destination), m.BuildSource(source)))
                .Concat(new[] {destination})
                .ToArray();

            return Expression.Lambda(
                Expression.Block(bindings),
                source,
                destination);
        }

        private static IEnumerable<PropertyInfo> FindMembers(string name, IEnumerable<PropertyInfo> sourceMembers)
        {
            var list = new List<PropertyInfo>();
            FindMembers(list, name, sourceMembers);
            return list;
        }

        private static void FindMembers(ICollection<PropertyInfo> list, string name,
                                        IEnumerable<PropertyInfo> sourceMembers)
        {
            if (String.IsNullOrEmpty(name))
                return;
            PropertyInfo propertyInfo =
                sourceMembers.FirstOrDefault(pi => name.StartsWith(pi.Name, StringComparison.InvariantCultureIgnoreCase));
            if (propertyInfo == null)
                throw new NotSupportedException();

            list.Add(propertyInfo);
            FindMembers(list, name.Substring(propertyInfo.Name.Length), propertyInfo.PropertyType.GetProperties());
        }
    }
}