using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Devices;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToHaus;

public class DevicesMapperTests
{
    private static readonly ZigbeeDeviceInfo ResolvableDeviceInfo = new("Philips", "929002335001");

    private static DevicesMapper CreateMapper(FakeZigbeeCoordinator coordinator)
    {
        var provider = ServiceProviderFactory.Create(zigbeeCoordinator: coordinator);
        return provider.GetRequiredService<DevicesMapper>();
    }

    [Fact]
    public async Task MapAsync_DeviceResolvesToAKnownType_UsesExternalIdConverterForId()
    {
        var address = new IeeeAddress(0x00124b0012345678);
        var coordinator = new FakeZigbeeCoordinator { DeviceInfoToReturn = ResolvableDeviceInfo };
        var device = new ZigbeeDevice(address, 0x1234, []);
        var mapper = CreateMapper(coordinator);

        var result = (await mapper.MapAsync([device], CancellationToken.None)).Single();

        Assert.Equal(ExternalIdConverter.ToExternalId(address), result.Id);
    }

    [Fact]
    public async Task MapAsync_DeviceResolvesToAKnownType_SetsTheResolvedDeviceType()
    {
        var coordinator = new FakeZigbeeCoordinator { DeviceInfoToReturn = ResolvableDeviceInfo };
        var device = new ZigbeeDevice(new IeeeAddress(1), 0x1234, []);
        var mapper = CreateMapper(coordinator);

        var result = (await mapper.MapAsync([device], CancellationToken.None)).Single();

        Assert.Equal(DeviceType.Light, result.DeviceType);
    }

    [Fact]
    public async Task MapAsync_DeviceResolvesToAKnownType_SetsNetworkAddress()
    {
        var coordinator = new FakeZigbeeCoordinator { DeviceInfoToReturn = ResolvableDeviceInfo };
        var device = new ZigbeeDevice(new IeeeAddress(1), 0x9abc, []);
        var mapper = CreateMapper(coordinator);

        var result = (await mapper.MapAsync([device], CancellationToken.None)).Single();

        Assert.Equal((ushort)0x9abc, result.NetworkAddress);
    }

    [Fact]
    public async Task MapAsync_MultipleDevicesEachResolveToAKnownType_ReturnsOneEventPerDevice()
    {
        var coordinator = new FakeZigbeeCoordinator { DeviceInfoToReturn = ResolvableDeviceInfo };
        var devices = new List<ZigbeeDevice>
        {
            new(new IeeeAddress(1), 1, []),
            new(new IeeeAddress(2), 2, []),
            new(new IeeeAddress(3), 3, []),
        };
        var mapper = CreateMapper(coordinator);

        var result = await mapper.MapAsync(devices, CancellationToken.None);

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task MapAsync_DeviceInfoResolvesToUnknown_DoesNotClobberWithAnUnknownDeviceTypeEvent()
    {
        var coordinator = new FakeZigbeeCoordinator { DeviceInfoToReturn = new ZigbeeDeviceInfo("nope", "nope") };
        var device = new ZigbeeDevice(new IeeeAddress(1), 0x1234, []);
        var mapper = CreateMapper(coordinator);

        var result = await mapper.MapAsync([device], CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task MapAsync_ReadDeviceInfoReturnsNull_DoesNotClobberWithAnUnknownDeviceTypeEvent()
    {
        var coordinator = new FakeZigbeeCoordinator { DeviceInfoToReturn = null };
        var device = new ZigbeeDevice(new IeeeAddress(1), 0x1234, []);
        var mapper = CreateMapper(coordinator);

        var result = await mapper.MapAsync([device], CancellationToken.None);

        Assert.Empty(result);
    }
}
