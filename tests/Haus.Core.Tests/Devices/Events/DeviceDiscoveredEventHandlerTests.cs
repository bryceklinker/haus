using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Events;

public class DeviceDiscoveredEventHandlerTest
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;

    public DeviceDiscoveredEventHandlerTest()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);
    }

    [Fact]
    public async Task WhenDeviceDiscoveredEventHandledThenAddsDeviceToDatabase()
    {
        var @event = RoutableEvent.FromEvent(new DeviceDiscoveredEvent("This is my external id"));

        await _hausBus.PublishAsync(@event);

        Assert.Equal(1, _context.Set<DeviceEntity>().Count());
    }

    [Fact]
    public async Task WhenDeviceDiscoveredMultipleTimesThenDeviceIsUpdatedFromEvent()
    {
        _context.AddDevice("three");

        var @event = RoutableEvent.FromEvent(
            new DeviceDiscoveredEvent("three", Metadata: [new MetadataModel("Model", "Help")])
        );
        await _hausBus.PublishAsync(@event);

        Assert.Equal(1, _context.Set<DeviceEntity>().Count());
        Assert.Contains(_context.Set<DeviceEntity>().Single().Metadata, m => m.Key == "Model" && m.Value == "Help");
    }

    [Fact]
    public async Task WhenDeviceDiscoveredCreatesANewDeviceThenDeviceCreatedEventPublished()
    {
        var @event = RoutableEvent.FromEvent(new DeviceDiscoveredEvent("idk", DeviceType.Light));

        await _hausBus.PublishAsync(@event);

        Assert.Single(_hausBus.GetPublishedRoutableEvents<DeviceCreatedEvent>());
        Assert.Empty(_hausBus.GetPublishedRoutableEvents<DeviceUpdatedEvent>());
    }

    [Fact]
    public async Task WhenDeviceDiscoveredUpdatesDeviceThenDeviceUpdatedEventPublished()
    {
        _context.AddDevice("idk");
        var @event = RoutableEvent.FromEvent(new DeviceDiscoveredEvent("idk", DeviceType.Light));

        await _hausBus.PublishAsync(@event);

        Assert.Single(_hausBus.GetPublishedRoutableEvents<DeviceUpdatedEvent>());
        Assert.Empty(_hausBus.GetPublishedRoutableEvents<DeviceCreatedEvent>());
    }
}
