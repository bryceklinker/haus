using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Commands;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands;

public class BackfillDeviceNetworkAddressesCommandHandlerTests
{
    private readonly HausDbContext _context;
    private readonly IHausBus _hausBus;

    public BackfillDeviceNetworkAddressesCommandHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.Create(_context);
    }

    [Fact]
    public async Task WhenDeviceHasLegacyNetworkAddressMetadataThenTypedNetworkAddressIsPopulated()
    {
        var device = _context.AddDevice(configure: d => d.AddOrUpdateMetadata("network_address", "4660"));

        await _hausBus.ExecuteCommandAsync(new BackfillDeviceNetworkAddressesCommand());

        Assert.Equal((ushort)4660, device.NetworkAddress);
    }

    [Fact]
    public async Task WhenBackfillRunsAgainThenNetworkAddressStaysCorrect()
    {
        var device = _context.AddDevice(configure: d => d.AddOrUpdateMetadata("network_address", "4660"));
        await _hausBus.ExecuteCommandAsync(new BackfillDeviceNetworkAddressesCommand());

        await _hausBus.ExecuteCommandAsync(new BackfillDeviceNetworkAddressesCommand());

        Assert.Equal((ushort)4660, device.NetworkAddress);
    }

    [Fact]
    public async Task WhenDeviceHasNoLegacyMetadataThenNetworkAddressRemainsNull()
    {
        var device = _context.AddDevice();

        await _hausBus.ExecuteCommandAsync(new BackfillDeviceNetworkAddressesCommand());

        Assert.Null(device.NetworkAddress);
    }

    [Fact]
    public async Task WhenDeviceAlreadyHasTypedNetworkAddressThenExistingValueIsNotOverwritten()
    {
        var device = _context.AddDevice(configure: d =>
        {
            d.NetworkAddress = 1;
            d.AddOrUpdateMetadata("network_address", "4660");
        });

        await _hausBus.ExecuteCommandAsync(new BackfillDeviceNetworkAddressesCommand());

        Assert.Equal((ushort)1, device.NetworkAddress);
    }
}
