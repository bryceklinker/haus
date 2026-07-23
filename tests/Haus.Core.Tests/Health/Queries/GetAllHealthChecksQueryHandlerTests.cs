using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Health.Queries;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Health.Queries;

public class GetAllHealthChecksQueryHandlerTests
{
    private readonly HausDbContext _context;
    private readonly IHausBus _bus;

    public GetAllHealthChecksQueryHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _bus = HausBusFactory.Create(_context);
    }

    [Fact]
    public async Task WhenGettingAllHealthChecksThenReturnsAllStoredHealthChecks()
    {
        _context.AddHealthCheck();
        _context.AddHealthCheck();
        _context.AddHealthCheck();

        var result = await _bus.ExecuteQueryAsync(new GetAllHealthChecksQuery());

        Assert.Equal(3, result.Count);
        Assert.Equal(3, result.Items.Length);
    }
}
