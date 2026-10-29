using System;

namespace Haus.Zigbee.Models.Diagnostics;

public record ZigbeeConnectionStatusModel(
    bool IsConnected,
    string? Reason,
    DateTimeOffset? ChangedAt,
    ZigbeeNetworkConfigModel? NetworkConfig
)
{
    public static readonly ZigbeeConnectionStatusModel Unknown = new(false, null, null, null);
}
