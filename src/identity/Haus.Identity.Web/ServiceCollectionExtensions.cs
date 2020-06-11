using System.Reflection;
using System.Security.Claims;
using Haus.Identity.Core;
using Haus.Identity.Core.Accounts.Models;
using Haus.Identity.Core.Common.Storage;
using Haus.Identity.Web.Common.Authorization;
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
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy(AuthorizationPolicies.AdminPolicyName, AuthorizationPolicies.Admin);
                opts.DefaultPolicy = AuthorizationPolicies.Default;
            });
            services.AddHausIdentityCore()
                .AddControllersWithViews()
                .AddRazorOptions(opts =>
                {
                    opts.ViewLocationFormats.Add("/{1}/Views/{0}.cshtml");
                    opts.ViewLocationFormats.Add("/Common/Views/{0}.cshtml");
                });

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
                })
                .AddDeveloperSigningCredential();
            
            services.AddSpaStaticFiles(opts => opts.RootPath = "client-app/build");
            return services;
        }
    }
}