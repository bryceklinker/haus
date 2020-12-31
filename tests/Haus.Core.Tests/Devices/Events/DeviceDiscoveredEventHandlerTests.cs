using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices.Discovery;
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
            var @event = RoutableEvent.FromEvent(new DeviceDiscoveredModel("This is my external id"));

            await _hausBus.PublishAsync(@event);

            Assert.Single(_context.Set<DeviceEntity>());
        }

        [Fact]
        public async Task WhenDeviceDiscoveredMultipleTimesThenDeviceIsUpdatedFromEvent()
        {
            _context.AddDevice("three");

            var @event = RoutableEvent.FromEvent(new DeviceDiscoveredModel("three", metadata: new[]
            {
                new MetadataModel("Model", "Help"),
            }));
            await _hausBus.PublishAsync(@event);

            Assert.Single(_context.Set<DeviceEntity>());
            
            var deviceEntity = _context.Set<DeviceEntity>().Single();
            Assert.Contains(deviceEntity.Metadata, m => m.Key == "Model" && m.Value == "Help");
        }
    }
}