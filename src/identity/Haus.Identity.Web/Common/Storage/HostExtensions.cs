using System.Linq;
using System.Threading.Tasks;
using Haus.Identity.Core.Common.Storage;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Identity.Web.Common.Storage
{
    public static class HostExtensions
    {
        public static async Task MigrateDatabasesAsync(this IHost host)
        {
            await host.MigrateDatabaseAsync<ConfigurationDbContext>();
            await host.MigrateDatabaseAsync<PersistedGrantDbContext>();
            await host.MigrateDatabaseAsync<HausIdentityDbContext>();
        }

        private static async Task MigrateDatabaseAsync<TContext>(this IHost host)
            where TContext : DbContext
        {
            using var scope = host.Services.CreateScope();
            await using var context = scope.ServiceProvider.GetRequiredService<TContext>();
            await context.Database.MigrateAsync();
        }
    }
}