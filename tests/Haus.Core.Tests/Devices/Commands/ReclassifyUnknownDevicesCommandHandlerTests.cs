using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Commands;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands;

public class ReclassifyUnknownDevicesCommandHandlerTests
{
    private readonly HausDbContext _context;
    private readonly IHausBus _hausBus;

    public ReclassifyUnknownDevicesCommandHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.Create(_context);
    }

    // Regression coverage for production devices 3 ('Underdesk Lighting Strip') and 4 ('Middle
    // Basement Light') stuck at DeviceType.Unknown -- their Vendor/Model metadata was already
    // stored from original pairing, but nothing ever re-ran classification against it without a
    // live Zigbee rediscovery.
    [Fact]
    public async Task WhenUnknownDeviceHasVendorAndModelThatNowResolveThenDeviceTypeIsUpgraded()
    {
        var device = _context.AddDevice(configure: d =>
        {
            d.AddOrUpdateMetadata("vendor", "Gledopto");
            d.AddOrUpdateMetadata("model", "GL-MC-001");
        });

        await _hausBus.ExecuteCommandAsync(new ReclassifyUnknownDevicesCommand());

        Assert.Equal(DeviceType.Light, device.DeviceType);
    }

    [Fact]
    public async Task WhenUnknownDeviceVendorAndModelDoNotResolveThenDeviceTypeRemainsUnknown()
    {
        var device = _context.AddDevice(configure: d =>
        {
            d.AddOrUpdateMetadata("vendor", "nope");
            d.AddOrUpdateMetadata("model", "nope");
        });

        await _hausBus.ExecuteCommandAsync(new ReclassifyUnknownDevicesCommand());

        Assert.Equal(DeviceType.Unknown, device.DeviceType);
    }

    [Fact]
    public async Task WhenDeviceIsAlreadyClassifiedThenItIsNotTouchedEvenIfVendorAndModelMatchADifferentType()
    {
        var device = _context.AddDevice(
            deviceType: DeviceType.Switch,
            configure: d =>
            {
                d.AddOrUpdateMetadata("vendor", "Gledopto");
                d.AddOrUpdateMetadata("model", "GL-MC-001");
            }
        );

        await _hausBus.ExecuteCommandAsync(new ReclassifyUnknownDevicesCommand());

        Assert.Equal(DeviceType.Switch, device.DeviceType);
    }

    [Fact]
    public async Task WhenReclassifiedDeviceBecomesALightThenChangesAreSavedToDatabase()
    {
        var device = _context.AddDevice(configure: d =>
        {
            d.AddOrUpdateMetadata("vendor", "Gledopto");
            d.AddOrUpdateMetadata("model", "GL-MC-001");
        });

        await _hausBus.ExecuteCommandAsync(new ReclassifyUnknownDevicesCommand());

        var updated = await _context.FindByIdAsync<DeviceEntity>(device.Id);
        Assert.Equal(DeviceType.Light, updated?.DeviceType);
        Assert.True(updated?.IsLight);
    }

    [Fact]
    public async Task WhenReclassifyRunsAgainThenDeviceTypeStaysCorrect()
    {
        var device = _context.AddDevice(configure: d =>
        {
            d.AddOrUpdateMetadata("vendor", "Gledopto");
            d.AddOrUpdateMetadata("model", "GL-MC-001");
        });
        await _hausBus.ExecuteCommandAsync(new ReclassifyUnknownDevicesCommand());

        await _hausBus.ExecuteCommandAsync(new ReclassifyUnknownDevicesCommand());

        Assert.Equal(DeviceType.Light, device.DeviceType);
    }

    [Fact]
    public async Task WhenDeviceHasNoVendorOrModelMetadataThenDeviceTypeRemainsUnknown()
    {
        var device = _context.AddDevice();

        await _hausBus.ExecuteCommandAsync(new ReclassifyUnknownDevicesCommand());

        Assert.Equal(DeviceType.Unknown, device.DeviceType);
    }
}
