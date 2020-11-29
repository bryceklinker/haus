using Newtonsoft.Json.Linq;

namespace Haus.Zigbee.Host.Tests.Support
{
    public class Zigbee2MqttMetaBuilder
    {
        private JObject _meta = new JObject();

        public Zigbee2MqttMetaBuilder WithModel(string model)
        {
            _meta.Add("model", model);
            return this;
        }

        public Zigbee2MqttMetaBuilder WithFriendlyName(string friendlyName)
        {
            _meta.Add("friendly_name", friendlyName);
            return this;
        }

        public Zigbee2MqttMetaBuilder WithDescription(string description)
        {
            _meta.Add("description", description);
            return this;
        }

        public Zigbee2MqttMetaBuilder WithVendor(string vendor)
        {
            _meta.Add("vendor", vendor);
            return this;
        }

        public Zigbee2MqttMetaBuilder WithSupported(bool supported)
        {
            _meta.Add("supported", supported);
            return this;
        }

        public JToken Build()
        {
            try
            {
                return _meta;
            }
            finally
            {
                _meta = new JObject();
            }
        }
    }
}