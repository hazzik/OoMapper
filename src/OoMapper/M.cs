using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    public class M
    {
        private readonly PropertyInfo destination;
        private readonly IEnumerable<PropertyInfo> source;

        public M(PropertyInfo destination, IEnumerable<PropertyInfo> source)
        {
            this.destination = destination;
            this.source = source;
        }

        public PropertyInfo Destination
        {
            get { return destination; }
        }

        private static Expression CreatePropertyExpression(Expression source, PropertyInfo sourceProperty,
                                                          Type destinationType)
        {
            MemberExpression property = Expression.Property(source, sourceProperty);

            if (destinationType.IsArray)
            {
                destinationType = destinationType.GetElementType();

                Type sourceType = GetElementType(sourceProperty.PropertyType);

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
            LambdaExpression mapper = Mapper.mappers[key].Build();
            return Expression.Call(typeof (Enumerable), "ToArray", new[] {destinationType},
                                   Expression.Call(typeof (Enumerable), "Select",
                                                   new[] {sourceType, destinationType},
                                                   property, mapper));
        }

        public Expression BuildSource(Expression x)
        {
            return source.Aggregate(x, (current, memberInfo) =>
                                       CreatePropertyExpression(current, memberInfo, Destination.PropertyType));
        }
    }
}