using System;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Haus.Core.Tests.Devices.Entities;

public class DeviceEndpointEntityConfigurationTests
{
    [Fact]
    public async Task WhenDeviceEndpointIsPersistedThenItRoundTripsThroughTheDatabase()
    {
        var databaseName = $"{Guid.NewGuid()}";
        var device = new DeviceEntity(externalId: "endpoint-device");
        device.Endpoints.Add(new DeviceEndpointEntity(1, [0x0006, 0x0008]) { Device = device });

        await using (var writeContext = CreateContext(databaseName))
        {
            writeContext.Add(device);
            await writeContext.SaveChangesAsync();
        }

        await using var readContext = CreateContext(databaseName);
        var persisted = await readContext
            .Set<DeviceEntity>()
            .Include(d => d.Endpoints)
            .AsNoTracking()
            .SingleAsync(d => d.ExternalId == "endpoint-device");

        var endpoint = Assert.Single(persisted.Endpoints);
        Assert.Equal((byte)1, endpoint.EndpointId);
        Assert.Equal(new ushort[] { 0x0006, 0x0008 }, endpoint.InClusters);
    }

    [Fact]
    public async Task WhenDeviceHasNoEndpointsThenPersistedDeviceHasAnEmptyEndpointsCollection()
    {
        var databaseName = $"{Guid.NewGuid()}";
        var device = new DeviceEntity(externalId: "no-endpoints-device");

        await using (var writeContext = CreateContext(databaseName))
        {
            writeContext.Add(device);
            await writeContext.SaveChangesAsync();
        }

        await using var readContext = CreateContext(databaseName);
        var persisted = await readContext
            .Set<DeviceEntity>()
            .Include(d => d.Endpoints)
            .AsNoTracking()
            .SingleAsync(d => d.ExternalId == "no-endpoints-device");

        Assert.Empty(persisted.Endpoints);
    }

    private static HausDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<HausDbContext>().UseInMemoryDatabase(databaseName).Options;
        return new HausDbContext(options);
    }
}
