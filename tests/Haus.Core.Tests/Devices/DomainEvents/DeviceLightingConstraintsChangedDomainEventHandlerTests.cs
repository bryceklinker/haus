using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Devices.DomainEvents;
using Haus.Core.Devices.Entities;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Devices.Events;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.DomainEvents
{
    public class DeviceLightingConstraintsChangedDomainEventHandlerTests
    {
        private readonly CapturingHausBus _hausBus;

        public DeviceLightingConstraintsChangedDomainEventHandlerTests()
        {
            _hausBus = HausBusFactory.CreateCapturingBus();
        }

        [Fact]
        public async Task WhenDeviceLightingConstraintsChangedDomainEventReceivedThenPublishesDeviceLightingConstraintsChangedEventPublished()
        {
            var constraints = LightingConstraintsEntity.Default;
            var device = new DeviceEntity();
            _hausBus.Enqueue(new DeviceLightingConstraintsChangedDomainEvent(device, constraints));

            await _hausBus.FlushAsync();

            _hausBus.GetPublishedRoutableEvents<DeviceLightingConstraintsChangedEvent>()
                .Should().HaveCount(1);
        }
    }
}