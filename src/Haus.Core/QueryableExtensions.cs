using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core
{
    public static class QueryableExtensions
    {
        public static async Task<ListResult<T>> ToListResultAsync<T>(
            this IQueryable<T> source,
            CancellationToken cancellationToken = default)
        {
            return new ListResult<T>(await source.ToArrayAsync(cancellationToken).ConfigureAwait(false));
        }
    }
}