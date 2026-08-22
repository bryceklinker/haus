using System;

namespace Haus.Core.Models.Zigbee;

public record ZigbeeConnectionStatusModel(bool IsConnected, string? Reason, DateTimeOffset? ChangedAt)
{
    public static readonly ZigbeeConnectionStatusModel Unknown = new(false, null, null);
}
