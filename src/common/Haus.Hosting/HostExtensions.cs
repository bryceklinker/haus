using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Hosting
{
    public static class HostExtensions
    {
        public static async Task MigrateDatabaseAsync<TContext>(this IHost host)
            where TContext : DbContext
        {
            using var scope = host.Services.CreateScope();
            await using var context = scope.ServiceProvider.GetRequiredService<TContext>();
            await context.Database.MigrateAsync();
        }

        public static bool IsNonProd(this IHostEnvironment env)
        {
            return !env.IsProduction();
        }
    }
}