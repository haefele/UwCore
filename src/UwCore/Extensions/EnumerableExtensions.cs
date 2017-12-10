using System.Collections.Generic;

namespace UwCore.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> self)
        {
            if (self == null)
                return new T[0];

            return self;
        }

        public static void AddRange<T>(this ICollection<T> self, IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                self.Add(item);
            }
        }
    }
}