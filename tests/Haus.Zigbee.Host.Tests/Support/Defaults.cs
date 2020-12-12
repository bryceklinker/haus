namespace Haus.Zigbee.Host.Tests.Support
{
    public static class Defaults
    {
        public static class HausOptions
        {
            public const string EventsTopic = "haus/events";
            public const string CommandsTopic = "haus/commands";
            public const string UnknownTopic = "haus/idk";
        }

        public static class ZigbeeOptions
        {
            public const string BaseTopic = "zigbee2mqtt";
        }
    }
}