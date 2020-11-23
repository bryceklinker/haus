using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Devices.Queries;
using Haus.Core.Tests.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Queries
{
    public class GetDevicesQueryHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly GetDevicesQueryHandler _handler;

        public GetDevicesQueryHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _handler = new GetDevicesQueryHandler(_context);
        }
        
        [Fact]
        public async Task WhenDevicesAreQueriedThenReturnsAllDevices()
        {
            _context.AddDevice("one");
            _context.AddDevice("two");
            _context.AddDevice("three");

            var result = await _handler.Handle(new GetDevicesQuery());

            Assert.Equal(3, result.Count);
            Assert.Contains(result.Items, i => i.ExternalId == "one");
            Assert.Contains(result.Items, i => i.ExternalId == "two");
            Assert.Contains(result.Items, i => i.ExternalId == "three");
        }
    }
}