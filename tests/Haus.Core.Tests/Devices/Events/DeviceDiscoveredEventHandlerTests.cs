using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Devices.Events;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Tests.Support;
using Haus.Cqrs;
using Haus.Testing.Support;
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
            var @event = RoutableEvent.FromEvent(new DeviceDiscoveredModel
            {
                Id = "This is my external id"
            });

            await _hausBus.PublishAsync(@event);

            Assert.Single(_context.Set<DeviceEntity>());
        }

        [Fact]
        public async Task WhenDeviceDiscoveredMultipleTimesThenDeviceIsUpdatedFromEvent()
        {
            _context.AddDevice("three");
            
            var @event = RoutableEvent.FromEvent(new DeviceDiscoveredModel
            {
                Id = "three",
                Metadata = new []
                {
                    new DeviceMetadataModel("Model", "Help"), 
                }
            });
            await _hausBus.PublishAsync(@event);

            Assert.Single(_context.Set<DeviceEntity>());
            
            var deviceEntity = _context.Set<DeviceEntity>().Single();
            Assert.Contains(deviceEntity.Metadata, m => m.Key == "Model" && m.Value == "Help");
        }
    }
}