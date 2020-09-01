using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        public static async Task ExecuteCommand(this IHost host, ICommand command)
        {
            using var scope = host.Services.CreateScope();
            var cqrsBus = scope.ServiceProvider.GetRequiredService<ICqrsBus>();
            await cqrsBus.ExecuteCommand(command);
        }

        public static async Task<TResult> ExecuteCommand<TResult>(this IHost host, ICommand<TResult> command)
        {
            using var scope = host.Services.CreateScope();
            var cqrsBus = scope.ServiceProvider.GetRequiredService<ICqrsBus>();
            return await cqrsBus.ExecuteCommand(command);
        }

        public static IConfiguration Configuration(this IHost host)
        {
            return host.Services.GetRequiredService<IConfiguration>();
        }
        
        public static bool IsNonProd(this IHostEnvironment env)
        {
            return !env.IsProduction();
        }
    }
}