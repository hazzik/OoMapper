using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    public static class Mapper
    {
        private static readonly IDictionary<Tuple<Type, Type>, LambdaExpression> mappers =
            new ConcurrentDictionary<Tuple<Type, Type>, LambdaExpression>();

        public static void Configure<TSource, TDestination>()
        {
            Type sourceType = typeof (TSource);
            Type destinationType = typeof (TDestination);

            PropertyInfo[] sourceMembers = sourceType.GetProperties();
            PropertyInfo[] destinationMembers = destinationType.GetProperties();

            Tuple<Type, Type> key = Tuple.Create(sourceType, destinationType);

            const string name = "src";

            ParameterExpression source = Expression.Parameter(sourceType, name);

            MemberAssignment[] bindings = destinationMembers
                .Select(x => new
                                 {
                                     Destination = x,
                                     Source = FindMember(x.PropertyType, x.Name, source, sourceMembers)
                                 })
                .Select(m => Expression.Bind(m.Destination, m.Source))
                .ToArray();

            Expression<Func<TSource, TDestination>> expression = Expression.Lambda<Func<TSource, TDestination>>(
                Expression.MemberInit(
                    Expression.New(destinationType), bindings), source);
            mappers.Add(key, expression);
        }

        private static Expression FindMember(Type destinationType, string name, Expression source,
                                             IEnumerable<PropertyInfo> sourceMembers)
        {
            var list = new List<PropertyInfo>();
            
            FindMembers(list, name, sourceMembers);

            return list.Aggregate(source, (current, memberInfo) => CreatePropertyExpression(current, memberInfo, destinationType));
        }

        private static void FindMembers(ICollection<PropertyInfo> list, string name, IEnumerable<PropertyInfo> sourceMembers)
        {
            if (string.IsNullOrEmpty(name))
                return;
            PropertyInfo propertyInfo = sourceMembers.FirstOrDefault(pi => name.StartsWith(pi.Name, StringComparison.InvariantCultureIgnoreCase));
            if (propertyInfo == null)
                throw new NotSupportedException();

            list.Add(propertyInfo);
            FindMembers(list, name.Substring(propertyInfo.Name.Length), propertyInfo.PropertyType.GetProperties());
        }

        private static Expression CreatePropertyExpression(Expression source, PropertyInfo sourceProperty,
                                                           Type destinationType)
        {
            MemberExpression property = Expression.Property(source, sourceProperty);

            if (destinationType.IsArray)
            {
                destinationType = destinationType.GetElementType();
                
                var sourceType = GetElementType(sourceProperty.PropertyType);

                Tuple<Type, Type> key = Tuple.Create(sourceType, destinationType);
                
                return CreateSelect(destinationType, property, sourceType, key);
            }
            return property;
        }

        private static Type GetElementType(Type propertyType)
        {
            if (propertyType.IsArray)
            {
                return propertyType.GetElementType();
            }
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
            {
                return propertyType.GetGenericArguments().First();
            }
            return null;
        }

        private static Expression CreateSelect(Type destinationType, Expression property, Type sourceType, Tuple<Type, Type> key)
        {
            LambdaExpression mapper = mappers[key];
            return Expression.Call(typeof(Enumerable), "ToArray", new[] { destinationType },
                                   Expression.Call(typeof(Enumerable), "Select",
                                                   new[] { sourceType, destinationType },
                                                   property, mapper));
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            Tuple<Type, Type> key = Tuple.Create(typeof (TSource), typeof (TDestination));
            var expression = (Expression<Func<TSource, TDestination>>) mappers[key];
            return expression.Compile().Invoke(source);
        }
    }
}