using System.Text.Json;
using Haus.Core.Models;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Common.Events
{
    public interface IRoutableEventFactory
    {
        RoutableEvent Create(byte[] bytes);
    }
    
    public class RoutableEventFactory : IRoutableEventFactory
    {
        public RoutableEvent Create(byte[] bytes)
        {
            if (!HausJsonSerializer.TryDeserialize(bytes, out HausEvent hausEvent))
                return null;
            
            return hausEvent.Type switch
            {
                DeviceDiscoveredModel.Type => CreateRoutableEvent<DeviceDiscoveredModel>(bytes),
                _ => null
            };
        }
        
        private RoutableEvent CreateRoutableEvent<T>(byte[] bytes)
        {
            var hausEvent = HausJsonSerializer.Deserialize<HausEvent<T>>(bytes);
            return new RoutableEvent<T>(hausEvent);
        }
    }
}