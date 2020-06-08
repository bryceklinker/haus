using System.Threading.Tasks;
using Haus.Identity.Core.Accounts;
using Haus.Identity.Core.Storage;
using IdentityServer4.EntityFramework.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Identity.Web.Storage
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

        public static async Task SeedDatabaseAsync(this IHost host)
        {
            await host.ExecuteRequest(new SeedAdminAccountRequest());
        }
        
        private static async Task ExecuteRequest<TRequest>(this IHost host, TRequest request)
            where TRequest : IRequest
        {
            using var scope = host.Services.CreateScope();
            var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediatr.Send(request);
        }
    }
}