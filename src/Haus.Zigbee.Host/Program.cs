using System;
using System.Threading.Tasks;
using Haus.Hosting;
using Haus.Zigbee.Host.Storage;
using Haus.Zigbee.Host.Zigbee;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Zigbee.Host;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<HausZigbeeDbContext>>();
            await using var context = await contextFactory.CreateDbContextAsync();
            await context.Database.MigrateAsync();

            var addressRegistry = scope.ServiceProvider.GetRequiredService<DeviceAddressRegistry>();
            await addressRegistry.LoadAsync();

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

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Microsoft
            .Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .UseHausLogging("Haus Zigbee")
            .UseSystemd()
            .UseWindowsService()
            .ConfigureServices(
                (ctx, services) =>
                {
                    services.AddHausZigbee(ctx.Configuration);
                }
            );
    }
}
