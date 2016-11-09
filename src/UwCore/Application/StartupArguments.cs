using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.Foundation;
using UwCore.Extensions;

namespace UwCore.Application
{
    public class StartupArguments
    {
        public static T Parse<T>(string arguments)
            where T : new()
        {
            var splitted = arguments.Split('?');

            if (splitted.Length != 2)
                return default(T);

            var expectedType = splitted[0];
            var queryString = splitted[1];

            if (typeof(T).Name != expectedType)
                return default(T);

            var result = new T();
            var decoder = new WwwFormUrlDecoder(queryString);
            result.InjectValues(decoder.ToDictionary(f => f.Name, f => (object)f.Value));

            return result;
        }

        public static string AsString<T>(T arguments)
        {
            string type = typeof(T).Name;

            var dictionary = BuildDictionary(arguments);
            var queryString = BuildQueryString(dictionary);

            return $"{type}{queryString}";
        }

        private static string BuildQueryString(Dictionary<string, string> valuesDictionary)
        {
            var valuesString = valuesDictionary
                .Aggregate("?", (current, pair) => current + (pair.Key + "=" + Uri.EscapeDataString(pair.Value) + "&"));
            valuesString = valuesString.Remove(valuesString.Length - 1);
            return valuesString;
        }

        private static Dictionary<string, string> BuildDictionary<T>(T arguments)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var property in typeof(T).GetProperties())
            {
                var value = property.GetValue(arguments);
                dictionary[property.Name] = value?.ToString();
            }

            return dictionary;
        }
    }
}