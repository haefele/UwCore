using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UwCore.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets a property by name, ignoring case and searching all interfaces.
        /// </summary>
        /// <param name="self">The type to inspect.</param>
        /// <param name="propertyName">The property to search for.</param>
        /// <returns>The property or null if not found.</returns>
        public static PropertyInfo GetPropertyCaseInsensitive(this Type self, string propertyName)
        {
            var typeInfo = self.GetTypeInfo();
            var typeList = new List<Type> { self };

            if (typeInfo.IsInterface)
            {
                typeList.AddRange(typeInfo.ImplementedInterfaces);
            }

            return typeList
                .Select(interfaceType => interfaceType.GetRuntimeProperty(propertyName))
                .FirstOrDefault(property => property != null);
        }
    }
}