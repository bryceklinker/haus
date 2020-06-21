using System;
using System.Collections.Generic;
using System.Linq;

namespace Haus.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsEmptyOrNull(this IEnumerable<string> source)
        {
            return source == null
                   || !source.Any();
        }
    }
}
