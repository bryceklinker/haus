using System;

namespace Haus.Zigbee.Host.Storage.Entities;

public class KnownZigbeeDeviceEntity
{
    public long Id { get; set; }
    public string ExternalId { get; set; } = "";
    public ushort NetworkAddress { get; set; }
    public string EndpointsJson { get; set; } = "[]";
    public bool IsSleepy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
