using System;
using System.Collections;
using System.Linq;

namespace OoMapper
{
    public static class TypeUtils
    {
        public static Type GetElementType(Type propertyType)
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

        public static bool IsEnumerable(this Type type)
        {
            return type.GetInterfaces().Contains(typeof (IEnumerable));
        }
    }
}