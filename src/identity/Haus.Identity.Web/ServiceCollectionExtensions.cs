using System.Reflection;
using Haus.Identity.Core;
using Haus.Identity.Core.Accounts.Models;
using Haus.Identity.Core.Common.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Identity.Web
{
    public static class ServiceCollectionExtensions
    {
        private static readonly string MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
        
        public static IServiceCollection AddHausIdentityWeb(
            this IServiceCollection services, 
            string connectionString)
        {
            services.AddHausIdentityCore()
                .AddControllers();

            services.AddDbContext<HausIdentityDbContext>(opts =>
            {
                opts.UseNpgsql(connectionString, b => b.MigrationsAssembly(MigrationsAssembly));
            });
                
            services.AddIdentityServer()
                .AddAspNetIdentity<HausUser>()
                .AddConfigurationStore(opts =>
                {
                    opts.ConfigureDbContext = dbOpts =>
                    {
                        dbOpts.UseNpgsql(connectionString, b => b.MigrationsAssembly(MigrationsAssembly));
                    };
                })
                .AddOperationalStore(opts =>
                {
                    opts.ConfigureDbContext = dbOpts =>
                    {
                        dbOpts.UseNpgsql(connectionString, b => b.MigrationsAssembly(MigrationsAssembly));
                    };
                });
            
            services.AddSpaStaticFiles(opts => opts.RootPath = "client-app/build");
            return services;
        }
    }
}