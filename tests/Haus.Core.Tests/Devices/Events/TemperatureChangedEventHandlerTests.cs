using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors.Temperature;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Events;

public class TemperatureChangedEventHandlerTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;
    private readonly DeviceEntity _sensor;

    public TemperatureChangedEventHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);
        _sensor = _context.AddDevice(deviceType: DeviceType.TemperatureSensor);
    }

    [Fact]
    public async Task WhenTemperatureChangedForKnownDeviceThenDeviceTemperatureIsUpdated()
    {
        var change = new TemperatureChangedModel(_sensor.ExternalId, 21.5);
        await _hausBus.PublishAsync(RoutableEvent.FromEvent(change));

        var updated = await _context.FindByIdAsync<DeviceEntity>(_sensor.Id);
        Assert.Equal(21.5, updated?.Temperature);
    }

    [Fact]
    public async Task WhenTemperatureChangedForUnknownDeviceThenNothingIsUpdated()
    {
        var change = new TemperatureChangedModel("unknown-device", 21.5);

        var act = () => _hausBus.PublishAsync(RoutableEvent.FromEvent(change));

        await act();
        Assert.False(_context.ChangeTracker.HasChanges());
    }
}
