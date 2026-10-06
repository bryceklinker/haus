using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors.Battery;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Events;

public class BatteryChangedEventHandlerTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;
    private readonly DeviceEntity _sensor;

    public BatteryChangedEventHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);
        _sensor = _context.AddDevice(deviceType: DeviceType.MotionSensor);
    }

    [Fact]
    public async Task WhenBatteryChangedForKnownDeviceThenDeviceBatteryLevelIsUpdated()
    {
        var change = new BatteryChangedModel(_sensor.ExternalId, 80);
        await _hausBus.PublishAsync(RoutableEvent.FromEvent(change));

        var updated = await _context.FindByIdAsync<DeviceEntity>(_sensor.Id);
        Assert.Equal(80, updated?.BatteryLevel);
    }

    [Fact]
    public async Task WhenBatteryChangedForUnknownDeviceThenNothingIsUpdated()
    {
        var change = new BatteryChangedModel("unknown-device", 80);

        var act = () => _hausBus.PublishAsync(RoutableEvent.FromEvent(change));

        await act();
        Assert.False(_context.ChangeTracker.HasChanges());
    }
}
