using System.Collections.Generic;
using System.Linq;

namespace Haus.Core
{
    public static class EnumerableExtensions
    {
        public static bool Missing<T>(this IEnumerable<T> source, T value)
        {
            return !source.Contains(value);
        }
    }
}