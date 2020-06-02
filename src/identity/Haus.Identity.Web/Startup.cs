using System.Reflection;
using Haus.Identity.Web.Account.Entities;
using Haus.Identity.Web.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Identity.Web
{
    public class Startup
    {
        private static readonly string MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name; 
        public IConfiguration Configuration { get; }

        private string ConnectionString => Configuration["DB_CONNECTION_STRING"];

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<HausIdentityDbContext>(opts =>
            {
                opts.UseNpgsql(ConnectionString, b => b.MigrationsAssembly(MigrationsAssembly));
            });
                
            services.AddIdentity<HausUser, HausRole>()
                .AddEntityFrameworkStores<HausIdentityDbContext>();
            
            services.AddControllersWithViews();

            services.AddIdentityServer()
                .AddAspNetIdentity<HausUser>()
                .AddConfigurationStore(opts =>
                {
                    opts.ConfigureDbContext = dbOpts =>
                    {
                        dbOpts.UseNpgsql(ConnectionString, b => b.MigrationsAssembly(MigrationsAssembly));
                    };
                })
                .AddOperationalStore(opts =>
                {
                    opts.ConfigureDbContext = dbOpts =>
                    {
                        dbOpts.UseNpgsql(ConnectionString, b => b.MigrationsAssembly(MigrationsAssembly));
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
