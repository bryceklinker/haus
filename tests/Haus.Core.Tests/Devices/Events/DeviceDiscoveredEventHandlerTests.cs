using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Devices.Events;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Tests.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Events
{
    public class DeviceDiscoveredEventHandlerTest
    {
        private readonly HausDbContext _context;
        private readonly IHausBus _hausBus;

        public DeviceDiscoveredEventHandlerTest()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.Create(_context);
        }

        [Fact]
        public async Task WhenDeviceDiscoveredEventHandledThenAddsDeviceToDatabase()
        {
            var @event = new RoutableEvent<DeviceDiscoveredModel>(new DeviceDiscoveredModel
            {
                Id = "This is my external id"
            }.AsHausEvent());

            await _hausBus.PublishAsync(@event);

            Assert.Single(_context.Set<DeviceEntity>());
        }

        [Fact]
        public async Task WhenDeviceDiscoveredMultipleTimesThenDeviceIsUpdatedFromEvent()
        {
            _context.AddDevice("three");
            
            var @event = new RoutableEvent<DeviceDiscoveredModel>(new DeviceDiscoveredModel
            {
                Id = "three",
                Model = "Help"
            }.AsHausEvent());
            await _hausBus.PublishAsync(@event);

            Assert.Single(_context.Set<DeviceEntity>());
            
            var deviceEntity = _context.Set<DeviceEntity>().Single();
            Assert.Equal("Model", deviceEntity.Metadata.First().Key);
            Assert.Equal("Help", deviceEntity.Metadata.First().Value);
        }
    }
}