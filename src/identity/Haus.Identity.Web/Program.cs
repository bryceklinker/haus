using System.Threading.Tasks;
using Haus.Hosting;
using Haus.Identity.Web.IdentityResources.Commands;
using Haus.Identity.Web.Users.Commands;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Haus.Identity.Web
{
    public class Program : HausProgram
    {
        public Program(string[] args) 
            : base("Haus Identity Web", args)
        {
        }

        public static async Task<int> Main(string[] args)
        {
            var program = new Program(args);
            return await program.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            GetHausHostBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        protected override void ConfigureHost(IHostBuilder builder)
        {
            builder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
        }

        protected override async Task BeforeRunAsync(IHost host)
        {
            var config = host.Configuration();
            await host.MigrateDatabasesAsync();
            await host.ExecuteCommand(new CreateDefaultIdentityResourcesCommand());
            await host.ExecuteCommand(new CreateUserCommand(config.AdminUsername(), config.AdminPassword()));
        }
    }
}
