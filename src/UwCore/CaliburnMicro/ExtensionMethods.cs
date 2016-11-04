﻿namespace Caliburn.Micro {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Generic extension methods used by the framework.
    /// </summary>
    public static class ExtensionMethods {

        /// <summary>
        /// Gets a property by name, ignoring case and searching all interfaces.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <param name="propertyName">The property to search for.</param>
        /// <returns>The property or null if not found.</returns>
        public static PropertyInfo GetPropertyCaseInsensitive(this Type type, string propertyName)
        {
#if WinRT
            var typeInfo = type.GetTypeInfo();
            var typeList = new List<Type> { type };

            if (typeInfo.IsInterface)
            {
                typeList.AddRange(typeInfo.ImplementedInterfaces);
            }

            return typeList
                .Select(interfaceType => interfaceType.GetRuntimeProperty(propertyName))
                .FirstOrDefault(property => property != null);
#else
            var typeList = new List<Type> { type };

            if (type.IsInterface) {
                typeList.AddRange(type.GetInterfaces());
            }

            var flags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

            if (IncludeStaticProperties) {
                flags = flags | BindingFlags.Static;
            }

            return typeList
                .Select(interfaceType => interfaceType.GetProperty(propertyName, flags))
                .FirstOrDefault(property => property != null);
#endif
        }

        /// <summary>
        /// Get's the name of the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The assembly's name.</returns>
        public static string GetAssemblyName(this Assembly assembly) {
            return assembly.FullName.Remove(assembly.FullName.IndexOf(','));
        }

        /// <summary>
        /// Gets all the attributes of a particular type.
        /// </summary>
        /// <typeparam name="T">The type of attributes to get.</typeparam>
        /// <param name="member">The member to inspect for attributes.</param>
        /// <param name="inherit">Whether or not to search for inherited attributes.</param>
        /// <returns>The list of attributes found.</returns>
        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member, bool inherit) {
#if WinRT || CORE
            return member.GetCustomAttributes(inherit).OfType<T>();
#else
            return Attribute.GetCustomAttributes(member, inherit).OfType<T>();
#endif
        }

#if WinRT || CORE
        /// <summary>
        /// Gets a collection of the public types defined in this assembly that are visible outside the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>A collection of the public types defined in this assembly that are visible outside the assembly.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<Type> GetExportedTypes(this Assembly assembly) {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            return assembly.ExportedTypes;
        }

        /// <summary>
        /// Returns a value that indicates whether the specified type can be assigned to the current type.
        /// </summary>
        /// <param name="target">The target type</param>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the specified type can be assigned to this type; otherwise, false.</returns>
        public static bool IsAssignableFrom(this Type target, Type type) {
            return target.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }
#endif

        /// <summary>
        /// Gets the value for a key. If the key does not exist, return default(TValue);
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary to call this method on.</param>
        /// <param name="key">The key to look up.</param>
        /// <returns>The key value. default(TValue) if this key is not in the dictionary.</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
            TValue result;
            return dictionary.TryGetValue(key, out result) ? result : default(TValue);
        }
    }
}