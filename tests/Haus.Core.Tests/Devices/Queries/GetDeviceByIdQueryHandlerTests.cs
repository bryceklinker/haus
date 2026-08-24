using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Queries;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Queries;

public class GetDeviceByIdQueryHandlerTests
{
    private readonly HausDbContext _context;
    private readonly IHausBus _hausBus;

    public GetDeviceByIdQueryHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.Create(_context);
    }

    [Fact]
    public async Task WhenIdMatchesDeviceThenReturnsPopulatedModel()
    {
        var device = _context.AddDevice();

        var model = await _hausBus.ExecuteQueryAsync(new GetDeviceByIdQuery(device.Id));

        Assert.Equal(device.Name, model.Name);
    }

    [Fact]
    public async Task WhenDeviceHasNetworkAddressThenReturnedModelIncludesIt()
    {
        var device = _context.AddDevice(configure: d => d.NetworkAddress = 0x9abc);

        var model = await _hausBus.ExecuteQueryAsync(new GetDeviceByIdQuery(device.Id));

        Assert.Equal((ushort)0x9abc, model.NetworkAddress);
    }

    [Fact]
    public async Task WhenIdDoesNotMatchDeviceThenReturnsNull()
    {
        var model = await _hausBus.ExecuteQueryAsync(new GetDeviceByIdQuery(long.MaxValue));

        Assert.Null(model);
    }
}
