﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Caliburn.Micro
{
    public class MessageBinder
    {

        /// <summary>
        /// Custom converters used by the framework registered by destination type for which they will be selected.
        /// The converter is passed the existing value to convert and a "context" object.
        /// </summary>
        public static readonly Dictionary<Type, Func<object, object, object>> CustomConverters =
            new Dictionary<Type, Func<object, object, object>>
            {
                {
                    typeof (DateTime), (value, context) => {
                        DateTime result;
                        DateTime.TryParse(value.ToString(), out result);
                        return result;
                    }
                }
            };

        /// <summary>
        /// Coerces the provided value to the destination type.
        /// </summary>
        /// <param name="destinationType">The destination type.</param>
        /// <param name="providedValue">The provided value.</param>
        /// <param name="context">An optional context value which can be used during conversion.</param>
        /// <returns>The coerced value.</returns>
        public static object CoerceValue(Type destinationType, object providedValue, object context)
        {
            if (providedValue == null)
            {
                return GetDefaultValue(destinationType);
            }

            var providedType = providedValue.GetType();
            if (destinationType.IsAssignableFrom(providedType))
            {
                return providedValue;
            }

            if (CustomConverters.ContainsKey(destinationType))
            {
                return CustomConverters[destinationType](providedValue, context);
            }

            try
            {
#if !WinRT && !XFORMS
                var converter = TypeDescriptor.GetConverter(destinationType);

                if (converter.CanConvertFrom(providedType)) {
                    return converter.ConvertFrom(providedValue);
                }

                converter = TypeDescriptor.GetConverter(providedType);

                if (converter.CanConvertTo(destinationType)) {
                    return converter.ConvertTo(providedValue, destinationType);
                }
#endif
#if WinRT || XFORMS
                if (destinationType.GetTypeInfo().IsEnum)
                {
#else
                if (destinationType.IsEnum) {
#endif
                    var stringValue = providedValue as string;
                    if (stringValue != null)
                    {
                        return Enum.Parse(destinationType, stringValue, true);
                    }

                    return Enum.ToObject(destinationType, providedValue);
                }

                if (typeof(Guid).IsAssignableFrom(destinationType))
                {
                    var stringValue = providedValue as string;
                    if (stringValue != null)
                    {
                        return new Guid(stringValue);
                    }
                }
            }
            catch
            {
                return GetDefaultValue(destinationType);
            }

            try
            {
                return Convert.ChangeType(providedValue, destinationType, CultureInfo.CurrentCulture);
            }
            catch
            {
                return GetDefaultValue(destinationType);
            }
        }


        /// <summary>
        /// Gets the default value for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The default value.</returns>
        public static object GetDefaultValue(Type type)
        {
#if WinRT || XFORMS
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsClass || typeInfo.IsInterface ? null : System.Activator.CreateInstance(type);
#else
            return type.IsClass || type.IsInterface ? null : Activator.CreateInstance(type);
#endif
        }
    }
}