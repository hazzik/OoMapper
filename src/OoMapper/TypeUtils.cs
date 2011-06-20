using System;
using System.Collections;
using System.Linq;

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
    }
}