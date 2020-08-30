using System;
using System.Threading.Tasks;
using Haus.Hosting;
using Haus.Portal.Web.Common.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Haus.Portal.Web
{
    public class Program : HausProgram
    {
        public Program(string[] args)
            : base("Haus Portal Web", args)
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
            builder.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }

        protected override async Task BeforeRunAsync(IHost host)
        {
            await host.MigrateDatabaseAsync<HausPortalDbContext>();
        }
    }
}