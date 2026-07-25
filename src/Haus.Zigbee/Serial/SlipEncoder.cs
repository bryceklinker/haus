using System.Collections.Generic;

namespace Haus.Zigbee.Serial;

public class SlipEncoder
{
    private const byte End = 0xC0;

    public byte[] Encode(byte[] frame)
    {
        var encoded = new List<byte> { End };
        encoded.AddRange(frame);
        encoded.Add(End);
        return encoded.ToArray();
    }
}
