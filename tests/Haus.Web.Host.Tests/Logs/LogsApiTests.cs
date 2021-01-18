using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Logs
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class LogsApiTests
    {
        private readonly IHausApiClient _client;

        public LogsApiTests(HausWebHostApplicationFactory factory)
        {
            _client = factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task WhenGettingLogsFromApiThenReturnsLogsFromLogFiles()
        {
            var logs = await _client.GetLogsAsync();

            logs.Count.Should().Be(25);
            logs.Items.Should().HaveCount(25);
        }
    }
}