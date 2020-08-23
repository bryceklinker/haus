using Haus.Identity.Web.Common.Storage;
using Haus.Identity.Web.Users.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Haus.Identity.Web
{
    public class Startup
    {
        private static readonly string MigrationsAssembly = typeof(Startup).Assembly.GetName().Name;
        private IConfiguration Config { get; }
        private string DbConnectionString => Config.GetValue<string>("DbConnectionString");

        public Startup(IConfiguration config)
        {
            Config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddCors(opts => opts.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            }));
            services.AddDbContext<HausIdentityDbContext>(opts =>
                {
                    opts.UseNpgsql(DbConnectionString, psqlOpts => psqlOpts.MigrationsAssembly(MigrationsAssembly));
                })
                .AddIdentity<HausUser, HausRole>()
                .AddEntityFrameworkStores<HausIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllersWithViews()
                .AddRazorOptions(opts =>
                {
                    opts.ViewLocationFormats.Add("/Shared/Views/{0}.cshtml");
                    opts.ViewLocationFormats.Add("/{1}/Views/{0}.cshtml");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddIdentityServer(opts => { opts.UserInteraction.LoginUrl = "/users/login"; })
                .AddAspNetIdentity<HausUser>()
                .AddConfigurationStore(opts =>
                {
                    opts.ConfigureDbContext = dbOpts => dbOpts.UseNpgsql(DbConnectionString,
                        psqlOpts => { psqlOpts.MigrationsAssembly(MigrationsAssembly); });
                })
                .AddOperationalStore(opts =>
                {
                    opts.ConfigureDbContext = dbOpts => dbOpts.UseNpgsql(DbConnectionString,
                        psqlOpts => { psqlOpts.MigrationsAssembly(MigrationsAssembly); });
                })
                .AddDeveloperSigningCredential();
            services.AddSpaStaticFiles(opts => opts.RootPath = "client-app/build");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsNonProd()) app.UseDeveloperExceptionPage();

            app.UseSerilogRequestLogging();
            app.UseCors()
                .UseRouting()
                .UseIdentityServer()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHealthChecks("/.health");
                })
                .UseStaticFiles();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "client-app";
                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer("start");
                }
            });
        }
    }
}