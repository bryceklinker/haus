using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Devices.Queries;
using Haus.Core.Tests.Support;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Queries
{
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

            Assert.Equal(3, result.Count);
            Assert.Contains(result.Items, i => i.ExternalId == "one");
            Assert.Contains(result.Items, i => i.ExternalId == "two");
            Assert.Contains(result.Items, i => i.ExternalId == "three");
        }
    }
}