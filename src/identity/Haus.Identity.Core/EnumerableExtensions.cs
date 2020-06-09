using System;
using System.Collections.Generic;
using System.Linq;

namespace Haus.Identity.Core
{
    public static class EnumerableExtensions
    {
        public static bool ContainsIgnoreCase(this IEnumerable<string> source, string item)
        {
            return source.Any(i => i.Equals(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}