using System.Collections.Generic;
using System.Linq;

namespace Haus.Core.Models
{
    public static class EnumerableExtensions
    {
        public static bool Missing<T>(this IEnumerable<T> source, T value)
        {
            return !source.Contains(value);
        }

        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }
    }
}