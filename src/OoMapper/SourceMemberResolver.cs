using System;
using System.Collections;
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

		public Expression BuildSource(Expression x, Type destinationType)
		{
			return source.Aggregate(x, (current, memberInfo) =>
			                           CreatePropertyExpression(current, memberInfo, destinationType));
		}

		private static Expression CreatePropertyExpression(Expression source, PropertyInfo sourceProperty,
		                                                   Type destinationType)
		{
			MemberExpression property = Expression.Property(source, sourceProperty);

			if (IsEnumerable(destinationType))
			{
				var isArray = destinationType.IsArray;
				destinationType = GetElementType(destinationType);
				Type sourceType = GetElementType(sourceProperty.PropertyType);
				if (sourceType == null) return property;
				Tuple<Type, Type> key = Tuple.Create(sourceType, destinationType);
				return CreateSelect(destinationType, property, sourceType, key, isArray ? "ToArray" : "ToList");
			}

			return property;
		}

		private static Type GetElementType(Type propertyType)
		{
			if (propertyType.HasElementType)
			{
				return propertyType.GetElementType();
			}
			if (propertyType.IsGenericType)
			{
				return propertyType.GetGenericArguments().First();
			}
			return null;
		}

		private static bool IsEnumerable(Type type)
		{
			return type.GetInterfaces().Contains(typeof (IEnumerable));
		}

		private static Expression CreateSelect(Type destinationType, Expression property, Type sourceType,
		                                       Tuple<Type, Type> key, string methodName)
		{
		    return Expression.Call(typeof (Enumerable), methodName, new[] {destinationType},
			                       Expression.Call(typeof (Enumerable), "Select",
			                                       new[] {sourceType, destinationType},
			                                       property, Mapper.BuildNew(key)));
		}
	}
}