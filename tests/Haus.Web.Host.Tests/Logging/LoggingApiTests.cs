using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Logging;

[Collection(HausWebHostCollectionFixture.Name)]
public class LoggingApiTests
{
    private readonly HausWebHostApplicationFactory _factory;

    public LoggingApiTests(HausWebHostApplicationFactory factory)
    {
        _factory = factory;
    }
}