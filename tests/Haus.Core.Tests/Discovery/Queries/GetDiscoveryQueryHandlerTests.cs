using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Storage;
using Haus.Core.Discovery.Queries;
using Haus.Core.Models.Discovery;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Discovery.Queries;

public class GetDiscoveryQueryHandlerTests
{
    private readonly HausDbContext _context;
    private readonly IHausBus _hausBus;

    public GetDiscoveryQueryHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.Create(_context);
    }

    [Fact]
    public async Task WhenGettingDiscoveryThenReturnsDiscoveryFromDatabase()
    {
        _context.AddDiscovery(DiscoveryState.Enabled);

        var model = await _hausBus.ExecuteQueryAsync(new GetDiscoveryQuery());

        model.State.Should().Be(DiscoveryState.Enabled);
    }

    [Fact]
    public async Task WhenGettingDiscoveryAndDiscoveryIsMissingThenReturnsNull()
    {
        var model = await _hausBus.ExecuteQueryAsync(new GetDiscoveryQuery());

        model.Should().BeNull();
    }
}