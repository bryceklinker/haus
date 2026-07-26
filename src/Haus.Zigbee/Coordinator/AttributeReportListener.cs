using System;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Zcl;

namespace Haus.Zigbee.Coordinator;

// Watches a poll loop's incoming APS indications for global ZCL attribute-report and
// read-attributes-response frames and surfaces each decoded attribute as a protocol-generic
// ZigbeeAttributeReport. Indications that are not one of those two global ZCL commands (ZDP
// responses, cluster-specific commands, other ZCL commands) are ignored here on purpose.
public sealed class AttributeReportListener
{
    private const byte ReportAttributesCommand = 0x0a;
    private const byte ReadAttributesResponseCommand = 0x01;

    public AttributeReportListener(ApsPollLoop pollLoop)
    {
        pollLoop.IndicationReceived += OnIndicationReceived;
    }

    public event EventHandler<ZigbeeAttributeReport>? AttributeReported;

    private void OnIndicationReceived(object? sender, ApsIndicationReceived received)
    {
        var indication = received.Indication;
        var decoding = ZclFrameHeaderCodec.Decode(indication.AsduPayload);
        if (decoding.Header.FrameType != ZclFrameType.Global)
            return;

        var payload = indication.AsduPayload.AsSpan(decoding.ByteLength);
        if (decoding.Header.CommandId == ReportAttributesCommand)
            RaiseReportAttributes(indication, payload);
        else if (decoding.Header.CommandId == ReadAttributesResponseCommand)
            RaiseReadAttributesResponse(indication, payload);
    }

    private void RaiseReportAttributes(ApsDataIndicationFrame indication, ReadOnlySpan<byte> payload)
    {
        var result = ZclAttributeReportParser.ParseReportAttributes(payload);
        foreach (var attribute in result.Attributes)
            Raise(indication, attribute.AttributeId, attribute.Value);
    }

    private void RaiseReadAttributesResponse(ApsDataIndicationFrame indication, ReadOnlySpan<byte> payload)
    {
        var result = ZclAttributeReportParser.ParseReadAttributesResponse(payload);
        foreach (var attribute in result.Attributes)
        {
            if (attribute.Value is { } value)
                Raise(indication, attribute.AttributeId, value);
        }
    }

    private void Raise(ApsDataIndicationFrame indication, ushort attributeId, ZclAttributeValue value)
    {
        var report = new ZigbeeAttributeReport(
            indication.SourceNwkAddress,
            indication.SourceEndpoint,
            indication.ClusterId,
            attributeId,
            value
        );
        AttributeReported?.Invoke(this, report);
    }
}
