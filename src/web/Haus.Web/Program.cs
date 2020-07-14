using System.Threading.Tasks;
using Haus.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Haus.Web
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var loggingProgram = new HausLoggingProgram(
                "Web",
                CreateHostBuilder,
                RunHost);
            return await loggingProgram.Main(args);
        }

        public static async Task RunHost(IHost host)
        {
            await host.RunAsync();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
