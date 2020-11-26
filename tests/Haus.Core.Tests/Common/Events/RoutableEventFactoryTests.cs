using System.Text.Json;
using Haus.Core.Common.Events;
using Haus.Core.Models;
using Haus.Core.Models.Devices.Discovery;
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
        public void WhenBytesDoesNotRepresentAHausEventThenReturnsNull()
        {
            var bytes = HausJsonSerializer.SerializeToBytes("this is data");

            var routableEvent = _factory.Create(bytes);

            Assert.Null(routableEvent);
        }
    }
}