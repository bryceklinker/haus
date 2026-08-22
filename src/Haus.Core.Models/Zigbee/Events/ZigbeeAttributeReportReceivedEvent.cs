using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Zigbee.Events;

public record ZigbeeAttributeReportReceivedEvent(
    ushort NetworkAddress,
    string? IeeeAddress,
    ushort ClusterId,
    ushort AttributeId,
    byte DataType,
    ulong RawValue,
    string? StringValue
) : IHausEventCreator<ZigbeeAttributeReportReceivedEvent>
{
    public const string Type = "zigbee_attribute_report_received";

    public HausEvent<ZigbeeAttributeReportReceivedEvent> AsHausEvent()
    {
        return new(Type, this);
    }
}
