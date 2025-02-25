using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Queries;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Queries;

public class GetDevicesQueryHandlerTests
{
    private readonly HausDbContext _context;
    private readonly IHausBus _hausBus;

    public GetDevicesQueryHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.Create(_context);
    }

    [Fact]
    public async Task WhenDevicesAreQueriedThenReturnsAllDevices()
    {
        _context.AddDevice("one");
        _context.AddDevice("two");
        _context.AddDevice("three");

        var result = await _hausBus.ExecuteQueryAsync(new GetDevicesQuery());

        result.Count.Should().Be(3);
        result
            .Items.Should()
            .HaveCount(3)
            .And.Contain(i => i.ExternalId == "one")
            .And.Contain(i => i.ExternalId == "two")
            .And.Contain(i => i.ExternalId == "three");
    }
}
