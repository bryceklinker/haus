using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Devices.Queries;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Queries;

public class GetZigbeeKnownDeviceSeedQueryHandlerTests
{
    private readonly HausDbContext _context;
    private readonly IHausBus _hausBus;

    public GetZigbeeKnownDeviceSeedQueryHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.Create(_context);
    }

    [Fact]
    public async Task WhenDeviceHasNetworkAddressThenItIsIncludedWithEndpointsAndIsSleepy()
    {
        _context.AddDevice(
            externalId: "device-1",
            configure: d =>
            {
                d.NetworkAddress = 4660;
                d.IsSleepy = true;
                d.Endpoints.Add(new DeviceEndpointEntity(1, [6, 8]) { Device = d });
            }
        );

        var result = await _hausBus.ExecuteQueryAsync(new GetZigbeeKnownDeviceSeedQuery());

        var entry = Assert.Single(result.Items);
        Assert.Equal("device-1", entry.ExternalId);
        Assert.Equal((ushort)4660, entry.NetworkAddress);
        Assert.True(entry.IsSleepy);
        var endpoint = Assert.Single(entry.Endpoints);
        Assert.Equal(1, endpoint.EndpointId);
        Assert.Equal(new ushort[] { 6, 8 }, endpoint.InClusters);
    }

    [Fact]
    public async Task WhenDeviceHasNoNetworkAddressThenItIsExcluded()
    {
        _context.AddDevice(externalId: "device-2");

        var result = await _hausBus.ExecuteQueryAsync(new GetZigbeeKnownDeviceSeedQuery());

        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task WhenNoDevicesCarryZigbeeDataThenResultIsEmpty()
    {
        var result = await _hausBus.ExecuteQueryAsync(new GetZigbeeKnownDeviceSeedQuery());

        Assert.Empty(result.Items);
    }
}
