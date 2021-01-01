using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Entities;
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

            _context.Set<DeviceEntity>().Should().HaveCount(1);
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

            _context.Set<DeviceEntity>().Should().HaveCount(1);
            _context.Set<DeviceEntity>().Single().Metadata
                .Should().ContainEquivalentOf(new Metadata("Model", "Help"));
        }
    }
}