using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Haus.Identity.Core.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Core
{
    public static class EnumerableExtensions
    {
        public static bool ContainsIgnoreCase(this IEnumerable<string> source, string item)
        {
            return source.Any(i => i.Equals(item, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task<ListModel<T>> ToListModelAsync<T>(this IQueryable<T> source)
        {
            var items = await source.ToArrayAsync();
            return items.ToListModel();
        }

        public static ListModel<T> ToListModel<T>(this IEnumerable<T> source)
        {
            return new ListModel<T>(source);
        }
    }
}