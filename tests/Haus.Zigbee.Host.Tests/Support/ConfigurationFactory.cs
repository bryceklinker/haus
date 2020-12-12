using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Haus.Zigbee.Host.Tests.Support
{
    public static class ConfigurationFactory
    {
        public const string DefaultZigbeeBaseTopic = Defaults.ZigbeeOptions.BaseTopic;
        public const string DefaultHausEventsTopic = Defaults.HausOptions.EventsTopic;
        public const string DefaultHausCommandsTopic = Defaults.HausOptions.CommandsTopic;
        public const string DefaultHausUnknownTopic = Defaults.HausOptions.UnknownTopic;
        
        public static IConfiguration CreateConfig(string zigbeeBaseTopic = DefaultZigbeeBaseTopic,
            string hausEventsTopic = DefaultHausEventsTopic,
            string hausCommandsTopic = DefaultHausCommandsTopic,
            string hausUnknownTopic = DefaultHausUnknownTopic)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new []
                {
                    new KeyValuePair<string, string>("Zigbee:OverwriteConfig", "true"), 
                    new KeyValuePair<string, string>("Zigbee:ConfigFile", "./zigbee_data/configuration.yaml"), 
                    new KeyValuePair<string, string>("Zigbee:DataDirectory", "./zigbee_data"), 
                    new KeyValuePair<string, string>("Zigbee:Config:Mqtt:BaseTopic", zigbeeBaseTopic), 
                    new KeyValuePair<string, string>("Haus:EventsTopic", hausEventsTopic), 
                    new KeyValuePair<string, string>("Haus:CommandsTopic", hausCommandsTopic), 
                    new KeyValuePair<string, string>("Haus:UnknownTopic", hausUnknownTopic), 
                })
                .Build();
        }
    }
}