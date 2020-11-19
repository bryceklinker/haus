using System;
using Haus.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Haus.Web.Host
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
                .UseHausLogging("Haus Web")
                .ConfigureWebHostDefaults(web =>
                {
                    web.UseStartup<Startup>();
                });
    }
}