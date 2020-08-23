using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Haus.Portal.Web
{
    public class Startup
    {
        private IConfiguration Config { get; }
        private string Authority => Config.GetValue<string>("Auth:Authority");

        public Startup(IConfiguration config)
        {
            Config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddControllers();
            services.AddAuthentication()
                .AddIdentityServerAuthentication(opts =>
                {
                    opts.Authority = Authority;
                });
            services.AddCors(opts =>
            {
                opts.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
            });
            services.AddSpaStaticFiles(opts => opts.RootPath = "client-app/build");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
