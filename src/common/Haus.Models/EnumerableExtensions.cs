using System.Collections.Generic;

namespace Haus.Models
{
    public static class EnumerableExtensions
    {
        public static ListModel<TItem> ToListModel<TItem>(this IEnumerable<TItem> source)
        {
            return new ListModel<TItem>(source);
        }
    }
}