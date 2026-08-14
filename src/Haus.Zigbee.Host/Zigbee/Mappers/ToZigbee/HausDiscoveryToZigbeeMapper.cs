using System;
using System.Linq;
using Haus.Core.Models.Discovery;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToZigbee;

public enum ZigbeeDiscoveryIntentType
{
    SetPermitJoin,
    SyncDevices,
}

public record ZigbeeDiscoveryIntent(ZigbeeDiscoveryIntentType Type, bool PermitJoinEnabled = false);

public class HausDiscoveryToZigbeeMapper
{
    private static readonly string[] SupportedTypes =
    {
        StartDiscoveryModel.Type,
        StopDiscoveryModel.Type,
        SyncDiscoveryModel.Type,
    };

    public bool IsSupported(string type)
    {
        return SupportedTypes.Contains(type);
    }

    public ZigbeeDiscoveryIntent Map(string commandType)
    {
        return commandType switch
        {
            StartDiscoveryModel.Type => new ZigbeeDiscoveryIntent(ZigbeeDiscoveryIntentType.SetPermitJoin, true),
            StopDiscoveryModel.Type => new ZigbeeDiscoveryIntent(ZigbeeDiscoveryIntentType.SetPermitJoin, false),
            SyncDiscoveryModel.Type => new ZigbeeDiscoveryIntent(ZigbeeDiscoveryIntentType.SyncDevices),
            _ => throw new InvalidOperationException($"Command with {commandType} is not supported"),
        };
    }
}
