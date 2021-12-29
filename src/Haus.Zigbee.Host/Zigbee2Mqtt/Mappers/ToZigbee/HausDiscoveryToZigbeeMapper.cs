using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haus.Core.Models;
using Haus.Core.Models.Discovery;
using Haus.Core.Models.ExternalMessages;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee;

public class HausDiscoveryToZigbeeMapper : IToZigbeeMapper
{
    private static readonly string[] SupportedTypes =
    {
        StartDiscoveryModel.Type,
        StopDiscoveryModel.Type,
        SyncDiscoveryModel.Type
    };

    private readonly IOptions<ZigbeeOptions> _options;

    public HausDiscoveryToZigbeeMapper(IOptions<ZigbeeOptions> options)
    {
        _options = options;
    }

    public bool IsSupported(string type)
    {
        return SupportedTypes.Contains(type);
    }

    public IEnumerable<MqttApplicationMessage> Map(MqttApplicationMessage message)
    {
        var command = HausJsonSerializer.Deserialize<HausCommand>(message.Payload);
        return command.Type switch
        {
            StartDiscoveryModel.Type => CreatePermitJoinMessage(true),
            StopDiscoveryModel.Type => CreatePermitJoinMessage(false),
            SyncDiscoveryModel.Type => CreateGetDevicesMessage(),
            _ => throw new InvalidOperationException($"Command with {command.Type} is not supported")
        };
    }

    private IEnumerable<MqttApplicationMessage> CreateGetDevicesMessage()
    {
        yield return new MqttApplicationMessage
        {
            Topic = $"{_options.GetBaseTopic()}/bridge/config/devices/get",
            Payload = Array.Empty<byte>()
        };
    }

    private IEnumerable<MqttApplicationMessage> CreatePermitJoinMessage(bool permitJoin)
    {
        yield return new MqttApplicationMessage
        {
            Topic = $"{_options.GetBaseTopic()}/bridge/config/permit_join",
            Payload = Encoding.UTF8.GetBytes(permitJoin.ToString().ToLowerInvariant())
        };
    }
}