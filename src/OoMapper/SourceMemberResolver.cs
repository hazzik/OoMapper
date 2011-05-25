using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
	public class SourceMemberResolver
	{
		private readonly IEnumerable<PropertyInfo> source;

		public SourceMemberResolver(IEnumerable<PropertyInfo> source)
		{
			this.source = source;
		}

		public IEnumerable<PropertyInfo> Source
		{
			get { return source; }
		}

		public Expression BuildSource(Expression x, Type destinationType)
		{
			return Source.Aggregate(x, (current, memberInfo) =>
			                           CreatePropertyExpression(current, memberInfo, destinationType));
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

		private static Expression CreateSelect(Type destinationType, Expression property, Type sourceType,
		                                       Tuple<Type, Type> key)
		{
			LambdaExpression mapper = Mapper.mappers[key].BuildNew();
			return Expression.Call(typeof (Enumerable), "ToArray", new[] {destinationType},
			                       Expression.Call(typeof (Enumerable), "Select",
			                                       new[] {sourceType, destinationType},
			                                       property, mapper));
		}
	}
}