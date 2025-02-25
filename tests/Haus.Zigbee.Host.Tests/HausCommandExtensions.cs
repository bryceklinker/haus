using System;
using Haus.Core.Models;
using Haus.Core.Models.ExternalMessages;
using MQTTnet;

namespace Haus.Zigbee.Host.Tests;

public static class HausCommandExtensions
{
    public static MqttApplicationMessage ToMqttMessage(this HausCommand command, string topic)
    {
        return new MqttApplicationMessage { Topic = topic, PayloadSegment = command.ToBytes() };
    }

    private static ArraySegment<byte> ToBytes(this HausCommand command)
    {
        return HausJsonSerializer.SerializeToBytes(command);
    }
}
