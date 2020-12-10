using Haus.Core.Common.Events;
using Haus.Core.Models;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.Devices.Sensors;
using Haus.Core.Models.Devices.Sensors.Motion;
using Xunit;

namespace Haus.Core.Tests.Common.Events
{
    public class RoutableHausEventFactoryTest
    {
        private readonly RoutableEventFactory _factory;

        public RoutableHausEventFactoryTest()
        {
            _factory = new RoutableEventFactory();
        }
        
        [Fact]
        public void WhenDeviceDiscoveredEventThenReturnsRoutableEventFromDeviceDiscovered()
        {
            var bytes = HausJsonSerializer.SerializeToBytes(new DeviceDiscoveredModel().AsHausEvent());
            
            var routableEvent = _factory.Create(bytes);
            
            Assert.IsType<RoutableEvent<DeviceDiscoveredModel>>(routableEvent);
        }

        [Fact]
        public void WhenMultiSensorChangedThenReturnsRoutableEventFromMultiSensorChanged()
        {
            var bytes = HausJsonSerializer.SerializeToBytes(new MultiSensorChanged().AsHausEvent());

            var routableEvent = _factory.Create(bytes);

            Assert.IsType<RoutableEvent<MultiSensorChanged>>(routableEvent);
        }

        [Fact]
        public void WhenMotionSensorChangedThenReturnsRoutableEventFromMotionSensorChanged()
        {
            var bytes = HausJsonSerializer.SerializeToBytes(new OccupancyChangedModel().AsHausEvent());

            var routableEvent = _factory.Create(bytes);

            Assert.IsType<RoutableEvent<OccupancyChangedModel>>(routableEvent);
        }

        [Fact]
        public void WhenBytesDoesNotRepresentAHausEventThenReturnsNull()
        {
            var bytes = HausJsonSerializer.SerializeToBytes("this is data");

            var routableEvent = _factory.Create(bytes);

            Assert.Null(routableEvent);
        }
    }
}