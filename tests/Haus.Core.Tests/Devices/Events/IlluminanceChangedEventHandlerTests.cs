using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors.Light;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Events;

public class IlluminanceChangedEventHandlerTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;
    private readonly DeviceEntity _sensor;

    public IlluminanceChangedEventHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);
        _sensor = _context.AddDevice(deviceType: DeviceType.LightSensor);
    }

    [Fact]
    public async Task WhenIlluminanceChangedForKnownDeviceThenDeviceIlluminanceIsUpdated()
    {
        var change = new IlluminanceChangedModel(_sensor.ExternalId, 123, 45);
        await _hausBus.PublishAsync(RoutableEvent.FromEvent(change));

        var updated = await _context.FindByIdAsync<DeviceEntity>(_sensor.Id);
        Assert.Equal(123, updated?.Illuminance);
        Assert.Equal(45, updated?.Lux);
    }

    [Fact]
    public async Task WhenIlluminanceChangedForUnknownDeviceThenNothingIsUpdated()
    {
        var change = new IlluminanceChangedModel("unknown-device", 123, 45);

        var act = () => _hausBus.PublishAsync(RoutableEvent.FromEvent(change));

        await act();
        Assert.False(_context.ChangeTracker.HasChanges());
    }
}
