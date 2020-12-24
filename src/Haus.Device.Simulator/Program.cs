using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Haus.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Haus.Device.Simulator
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
            Host.CreateDefaultBuilder(args)
                .UseHausLogging("Haus Device Simulator")
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
