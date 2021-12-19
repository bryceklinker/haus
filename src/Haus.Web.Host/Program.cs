using System;
using System.Threading.Tasks;
using Haus.Core.Common.Commands;
using Haus.Cqrs;
using Haus.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Web.Host
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();
                using var scope = host.Services.CreateScope();
                var bus = scope.GetService<IHausBus>();
                await bus.ExecuteCommandAsync(new InitializeCommand());
                await host.RunAsync();
                return 0;
            }
            catch (Exception e)
            {
                HausLogger.Fatal(e, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                HausLogger.EnsureFlushed();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseHausLogging("Haus Web")
                .UseSystemd()
                .UseWindowsService()
                .ConfigureWebHostDefaults(web =>
                {
                    web.UseKestrel();
                    web.UseStartup<Startup>();
                });
    }
}