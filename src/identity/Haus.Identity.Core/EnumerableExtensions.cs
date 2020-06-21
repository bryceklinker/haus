using System.Linq;
using System.Threading.Tasks;
using Haus.Models;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Core
{
    public static class EnumerableExtensions
    {
        public static async Task<ListModel<T>> ToListModelAsync<T>(this IQueryable<T> source)
        {
            var items = await source.ToArrayAsync();
            return items.ToListModel();
        }
    }
}