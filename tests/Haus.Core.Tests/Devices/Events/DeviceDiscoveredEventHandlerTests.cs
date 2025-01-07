using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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

        _context.Set<DeviceEntity>().Should().HaveCount(1);
    }

    [Fact]
    public async Task WhenDeviceDiscoveredMultipleTimesThenDeviceIsUpdatedFromEvent()
    {
        _context.AddDevice("three");

        var @event = RoutableEvent.FromEvent(new DeviceDiscoveredEvent("three", Metadata:
        [
            new MetadataModel("Model", "Help")
        ]));
        await _hausBus.PublishAsync(@event);

        _context.Set<DeviceEntity>().Should().HaveCount(1);
        _context.Set<DeviceEntity>().Single().Metadata
            .Should().ContainEquivalentOf(new DeviceMetadataEntity("Model", "Help"), opts =>
                opts.Excluding(m => m.Device).Excluding(m => m.Id));
    }

    [Fact]
    public async Task WhenDeviceDiscoveredCreatesANewDeviceThenDeviceCreatedEventPublished()
    {
        var @event = RoutableEvent.FromEvent(new DeviceDiscoveredEvent("idk", DeviceType.Light));

        await _hausBus.PublishAsync(@event);

        _hausBus.GetPublishedRoutableEvents<DeviceCreatedEvent>().Should().HaveCount(1);
        _hausBus.GetPublishedRoutableEvents<DeviceUpdatedEvent>().Should().BeEmpty();
    }

    [Fact]
    public async Task WhenDeviceDiscoveredUpdatesDeviceThenDeviceUpdatedEventPublished()
    {
        _context.AddDevice("idk");
        var @event = RoutableEvent.FromEvent(new DeviceDiscoveredEvent("idk", DeviceType.Light));

        await _hausBus.PublishAsync(@event);

        _hausBus.GetPublishedRoutableEvents<DeviceUpdatedEvent>().Should().HaveCount(1);
        _hausBus.GetPublishedRoutableEvents<DeviceCreatedEvent>().Should().BeEmpty();
    }
}