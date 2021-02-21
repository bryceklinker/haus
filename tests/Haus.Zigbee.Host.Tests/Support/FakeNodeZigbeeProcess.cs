using Haus.Zigbee.Host.Zigbee2Mqtt.Node;

namespace Haus.Zigbee.Host.Tests.Support
{
    public class FakeNodeZigbeeProcess : INodeZigbeeProcess
    {
        public void Dispose()
        {
            
        }

        public bool IsRunning { get; set; }
        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}