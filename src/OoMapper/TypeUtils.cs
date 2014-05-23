using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace OoMapper
{
	using System.Collections.Generic;

	public static class TypeUtils
    {
        public static Type GetElementTypeOfEnumerable(Type type)
        {
            if (type.HasElementType)
            {
                return type.GetElementType();
            }
            Type iEnumerable = type.GetInterface("IEnumerable`1", true);
            if (iEnumerable != null)
            {
                return iEnumerable.GetGenericArguments().First();
            }
            if (type.IsGenericType)
            {
                return type.GetGenericArguments().First();
            }             
            return null;
        }

        public static bool IsEnumerable(this Type type)
        {
            return type.GetInterfaces().Contains(typeof (IEnumerable));
        }

		public static bool IsDictionary(this Type type)
		{
			if(type.IsGenericType && type.GetGenericTypeDefinition()==typeof(IDictionary<,>))
				return true;
			return type.GetInterface("IDictionary`2", true) != null;
		}

	    public static bool IsNullable(this Type type)
	    {
	        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
	    }

	    public static IEnumerable<MemberInfo> GetWritableMembers(this Type type)
	    {
	        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

	        foreach (PropertyInfo propertyInfo in type.GetProperties(flags)
	            .Where(pi => pi.CanWrite)
	            .Where(pi => pi.GetIndexParameters().Length == 0))
	            yield return propertyInfo;

	        foreach (FieldInfo fieldInfo in type.GetFields(flags))
	            yield return fieldInfo;
	    }

        public static IEnumerable<MemberInfo> GetReadableMembers(this Type type)
	    {
	        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

	        var members = new List<MemberInfo>();
	        members.AddRange(type.GetProperties(flags)
	            .Where(pi => pi.CanRead)
	            .Where(pi => pi.GetIndexParameters().Length == 0));

	        members.AddRange(type.GetFields(flags));

            members.AddRange(type.GetMethods(flags)
                .Where(mi => mi.GetParameters().Length == 0)
                .Where(mi => mi.ReturnType != typeof (void)));
	        return members;
	    }
    }
}