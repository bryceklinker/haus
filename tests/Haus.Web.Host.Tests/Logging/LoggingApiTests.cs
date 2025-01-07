using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Logging;

[Collection(HausWebHostCollectionFixture.Name)]
public class LoggingApiTests(HausWebHostApplicationFactory factory)
{
    private readonly HausWebHostApplicationFactory _factory = factory;
}