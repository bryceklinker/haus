using Haus.Cqrs;
using Haus.Portal.Web.Common.Storage;
using Haus.ServiceBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Haus.Portal.Web
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
            services.AddControllers();
            services.AddHausServiceBus(Config, typeof(Startup).Assembly);
            services.AddHausCqrs(typeof(Startup).Assembly);
            services.AddAuthentication()
                .AddIdentityServerAuthentication(opts =>
                {
                    opts.Authority = Config.AuthorityUrl();
                });
            services.AddDbContext<HausPortalDbContext>(opts =>
                opts.UseNpgsql(DbConnectionString,
                    builder => builder.MigrationsAssembly(MigrationsAssembly))
            );
            services.AddCors(opts =>
            {
                opts.AddDefaultPolicy(policy =>
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
                        .AllowCredentials());
            });
            services.AddSpaStaticFiles(opts => opts.RootPath = "client-app/build");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection()
                .UseRouting()
                .UseStaticFiles()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks("/.health");
                    endpoints.MapControllers();
                })
                .UseSpaStaticFiles();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "client-app";
                if (env.IsDevelopment()) spa.UseReactDevelopmentServer("start");
            });
        }
    }
}