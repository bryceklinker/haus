using System.Threading.Tasks;
using Haus.Hosting;
using Haus.Identity.Web.Common.Storage;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Identity.Web
{
    public static class HostExtensions
    {
        public static async Task MigrateDatabasesAsync(this IHost host)
        {
            await host.MigrateDatabaseAsync<HausIdentityDbContext>();
            await host.MigrateDatabaseAsync<PersistedGrantDbContext>();
            await host.MigrateDatabaseAsync<ConfigurationDbContext>();
        }
    }
}