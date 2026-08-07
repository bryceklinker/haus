using System;
using System.Collections.Generic;
using Haus.Zigbee.Serial;

namespace Haus.Zigbee.Zcl;

public record ZclReadAttributesRequest(byte TransactionSequenceNumber, IReadOnlyList<ushort> AttributeIds);

public static class ZclReadAttributesRequestCodec
{
    private const byte ReadAttributesCommandId = 0x00;

    public static byte[] Encode(ZclReadAttributesRequest request)
    {
        var header = new ZclFrameHeader(
            ZclFrameType.Global,
            ZclDirection.ClientToServer,
            DisableDefaultResponse: false,
            request.TransactionSequenceNumber,
            ReadAttributesCommandId
        );

        var bytes = new List<byte>(ZclFrameHeaderCodec.Encode(header));
        foreach (var attributeId in request.AttributeIds)
            LittleEndian.Write(bytes, attributeId);

        return bytes.ToArray();
    }
}
