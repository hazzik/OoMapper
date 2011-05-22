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

        private static Expression FindMember(Type destinationType, string name, Expression source, PropertyInfo[] sourceMembers)
        {
            PropertyInfo propertyInfo = sourceMembers.FirstOrDefault(pi => string.Equals(pi.Name, name, StringComparison.InvariantCultureIgnoreCase));
            if (propertyInfo != null)
            {
                return CreatePropertyExpression(source, propertyInfo, destinationType);
            }
            PropertyInfo propertyInfo2 = sourceMembers.FirstOrDefault(pi => name.StartsWith(pi.Name, StringComparison.InvariantCultureIgnoreCase));
            if (propertyInfo2 != null)
            {
                return FindMember(destinationType, name.Substring(propertyInfo2.Name.Length), CreatePropertyExpression(source, propertyInfo2, destinationType), propertyInfo2.PropertyType.GetProperties());
            }
            throw new NotSupportedException();
        }

        private static Expression CreatePropertyExpression(Expression source, PropertyInfo sourceProperty, Type destinationType)
        {
            Type sourceType = sourceProperty.PropertyType;

            MemberExpression property = Expression.Property(source, sourceProperty);
            if (destinationType == sourceType)
                return property;

            if (destinationType.IsArray && sourceType.IsArray)
            {
                destinationType = destinationType.GetElementType();
                sourceType = sourceType.GetElementType();

                Tuple<Type, Type> key = Tuple.Create(sourceType, destinationType);
                LambdaExpression mapper = mappers[key];

                return Expression.Call(typeof (Enumerable), "ToArray", new[] {destinationType},
                                       Expression.Call(typeof (Enumerable), "Select", new[] {sourceType, destinationType},
                                                       property, mapper));
            }
            throw new NotSupportedException();
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            Tuple<Type, Type> key = Tuple.Create(typeof (TSource), typeof (TDestination));
            var expression = (Expression<Func<TSource, TDestination>>) mappers[key];
            return expression.Compile().Invoke(source);
        }
    }
}
