using System;
using System.Collections.Generic;
using System.IO;
using Haus.Mqtt.Client.Wrappers;
using Haus.Testing.Support.Fakes;
using Haus.Zigbee;
using Haus.Zigbee.Host;
using Haus.Zigbee.Host.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Haus.Zigbee.Host.Tests.Support;

public class HausZigbeeHostApplicationFactory : WebApplicationFactory<Startup>
{
    // Each factory gets its own on-disk Sqlite file: EF's AddDbContextFactory TryAdds its factory,
    // so an InMemory swap after the fact would be ignored -- a unique real file is both isolated
    // and exercises the production provider plus its migration.
    private readonly string _databasePath = Path.Combine(
        Path.GetTempPath(),
        $"haus-zigbee-tests-{Guid.NewGuid():N}.db"
    );

    public FakeZigbeeCoordinator Coordinator { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Acceptance");
        builder.ConfigureAppConfiguration(
            (_, config) =>
                config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Haus:Server"] = "mqtt://localhost:1883",
                        ["Zigbee:SerialPort"] = "/dev/ttyACM0",
                        ["Database:ConnectionString"] = $"Data Source={_databasePath}",
                    }
                )
        );
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IZigbeeCoordinator>();
            services.AddSingleton<IZigbeeCoordinator>(Coordinator);
            services.RemoveAll<IMqttFactory>();
            services.AddSingleton<IMqttFactory>(new FakeMqttClientFactory());
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<HausZigbeeDbContext>>();
        using var context = contextFactory.CreateDbContext();
        context.Database.Migrate();
        return host;
    }

    public IKnownZigbeeDeviceStore GetKnownDeviceStore()
    {
        return Services.GetRequiredService<IKnownZigbeeDeviceStore>();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && File.Exists(_databasePath))
            File.Delete(_databasePath);
    }
}
