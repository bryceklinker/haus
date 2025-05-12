using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models.Common;

namespace Haus.Core.Models;

public static class EnumerableExtensions
{
    public static ListResult<T> ToListResult<T>(this IEnumerable<T> source)
    {
        return new ListResult<T>(source.ToArray());
    }

    public static bool Missing<T>(this IEnumerable<T> source, T value)
    {
        return !source.Contains(value);
    }

    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
        return !source.Any();
    }
}
