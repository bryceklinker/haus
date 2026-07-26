using Haus.Zigbee.Zcl;

namespace Haus.Zigbee;

// Protocol-generic attribute report surfaced from an incoming ZCL attribute-report or
// read-attributes-response frame. It carries only addressing plus the raw ZCL attribute
// value; giving those attributes sensor meaning (illuminance, battery, ...) is the job of
// a higher layer, not this protocol type.
public sealed record ZigbeeAttributeReport(
    ushort SourceNwkAddress,
    byte Endpoint,
    ushort ClusterId,
    ushort AttributeId,
    ZclAttributeValue Value
);
