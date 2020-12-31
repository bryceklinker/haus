using System;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Models.Devices.Sensors;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Tests.Support;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Events
{
    public class MultiSensorChangedEventHandlerTests
    {
        private readonly CapturingHausBus _hausBus;

        public MultiSensorChangedEventHandlerTests()
        {
            _hausBus = HausBusFactory.CreateCapturingBus();
        }

        [Fact]
        public async Task WhenMultiSensorChangedHasOccupancyThenPublishesOccupancyChangedEvent()
        {
            var deviceId = $"{Guid.NewGuid()}";
            var change = new MultiSensorChanged
            {
                DeviceId = deviceId,
                OccupancyChanged = new OccupancyChangedModel(deviceId)
            };
            await _hausBus.PublishAsync(RoutableEvent.FromEvent(change));

            Assert.Single(_hausBus.GetPublishedEvents<RoutableEvent<OccupancyChangedModel>>());
        }
    }
}