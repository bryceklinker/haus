namespace Haus.Acceptance.Tests.Support.Zigbee2Mqtt;

public static class Zigbee2MqttTopics
{
    public const string Base = "zigbee2mqtt";
    public const string State = $"{Base}/bridge/state";
    public const string Log = $"{Base}/bridge/log";
    public const string Config = $"{Base}/bridge/config";
    public const string Devices = $"{Base}/bridge/config/devices";
}
