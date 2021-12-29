using Newtonsoft.Json.Linq;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Models;

public class Zigbee2MqttMeta
{
    public JObject Root { get; }

    public string Description => Root.Value<string>("description");
    public string FriendlyName => Root.Value<string>("friendly_name");
    public string Model => Root.Value<string>("model");
    public bool Supported => Root.Value<bool>("supported");
    public string Vendor => Root.Value<string>("vendor");

    public Zigbee2MqttMeta(JObject root)
    {
        Root = root;
    }
}