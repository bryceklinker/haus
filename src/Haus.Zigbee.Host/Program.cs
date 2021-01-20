using System;
using Haus.Hosting;
using Microsoft.Extensions.Hosting;

namespace Haus.Zigbee.Host
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
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
                .UseHausLogging("Haus Zigbee")
                .UseSystemd()
                .UseWindowsService()
                .ConfigureServices((ctx, services) =>
                {
                    services.AddHausZigbee(ctx.Configuration);
                });
    }
}