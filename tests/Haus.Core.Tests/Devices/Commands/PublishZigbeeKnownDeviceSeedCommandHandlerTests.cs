using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Commands;
using Haus.Core.Devices.Entities;
using Haus.Core.Devices.Events;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands;

public class PublishZigbeeKnownDeviceSeedCommandHandlerTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;

    public PublishZigbeeKnownDeviceSeedCommandHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);
    }

    [Fact]
    public async Task WhenDeviceCarriesZigbeeDataThenRequestsSeedPublishWithThatDevice()
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

        await _hausBus.ExecuteCommandAsync(new PublishZigbeeKnownDeviceSeedCommand());

        var requested = Assert.Single(_hausBus.GetPublishedEvents<ZigbeeKnownDeviceSeedRequestedEvent>());
        var entry = Assert.Single(requested.Seed.Devices);
        Assert.Equal("device-1", entry.ExternalId);
        Assert.Equal((ushort)4660, entry.NetworkAddress);
        Assert.True(entry.IsSleepy);
    }

    [Fact]
    public async Task WhenNoDeviceCarriesZigbeeDataThenRequestsSeedPublishWithEmptyList()
    {
        await _hausBus.ExecuteCommandAsync(new PublishZigbeeKnownDeviceSeedCommand());

        var requested = Assert.Single(_hausBus.GetPublishedEvents<ZigbeeKnownDeviceSeedRequestedEvent>());
        Assert.Empty(requested.Seed.Devices);
    }
}
