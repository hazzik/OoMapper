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

        public class M
        {
            public PropertyInfo Destination { get; set; }
            public Func<ParameterExpression, Expression> Source { get; set; }
        }

        public static void Configure<TSource, TDestination>()
        {
            PropertyInfo[] sourceMembers = typeof (TSource).GetProperties();
            PropertyInfo[] destinationMembers = typeof (TDestination).GetProperties();

            Tuple<Type, Type> key = Tuple.Create(typeof (TSource), typeof (TDestination));

            const string name = "src";

            ParameterExpression parameterExpression = Expression.Parameter(typeof (TSource), name);

            MemberAssignment[] bindings = destinationMembers
                .Select(x => new M {Destination = x, Source = FindMember(sourceMembers, x.Name, p => p)})
                .Select(m => Expression.Bind(m.Destination, m.Source(parameterExpression)))
                .ToArray();
            
            Expression<Func<TSource, TDestination>> expression = Expression.Lambda<Func<TSource, TDestination>>(
                Expression.MemberInit(
                    Expression.New(typeof (TDestination)), bindings), parameterExpression);
            mappers.Add(key, expression);
        }

        private static Func<ParameterExpression, Expression> FindMember(PropertyInfo[] sourceMembers, string name, Func<ParameterExpression, Expression> parameterExpression)
        {
            PropertyInfo propertyInfo = sourceMembers.FirstOrDefault(pi => string.Equals(pi.Name, name, StringComparison.InvariantCultureIgnoreCase));
            if (propertyInfo != null)
            {
                return CreatePropertyExpression(parameterExpression, propertyInfo);
            }
            PropertyInfo propertyInfo2 = sourceMembers.FirstOrDefault(pi => name.StartsWith(pi.Name, StringComparison.InvariantCultureIgnoreCase));
            if (propertyInfo2 != null)
            {
                return FindMember(propertyInfo2.PropertyType.GetProperties(),
                                  name.Substring(propertyInfo2.Name.Length),
                                  CreatePropertyExpression(parameterExpression, propertyInfo2));
            }
            throw new NotSupportedException();
        }

        private static Func<ParameterExpression, Expression> CreatePropertyExpression(Func<ParameterExpression, Expression> parameterExpression, PropertyInfo propertyInfo)
        {
            return p => Expression.Property(parameterExpression(p), propertyInfo);
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            Tuple<Type, Type> key = Tuple.Create(typeof (TSource), typeof (TDestination));
            var expression = (Expression<Func<TSource, TDestination>>) mappers[key];
            return expression.Compile().Invoke(source);
        }
    }
}
