using System.Collections.Generic;

namespace Haus.Zigbee.Serial;

// SLIP framing (RFC 1055): each frame is preceded and followed by an END delimiter;
// literal END/ESC bytes in the payload are byte-stuffed with the ESC escape sequences.
public class SlipEncoder
{
    private const byte End = 0xC0;
    private const byte Esc = 0xDB;
    private const byte EscEnd = 0xDC;
    private const byte EscEsc = 0xDD;

    public byte[] Encode(byte[] frame)
    {
        var encoded = new List<byte> { End };
        foreach (var value in frame)
            encoded.AddRange(Stuff(value));
        encoded.Add(End);
        return encoded.ToArray();
    }

    private static IEnumerable<byte> Stuff(byte value)
    {
        return value switch
        {
            End => new byte[] { Esc, EscEnd },
            Esc => new byte[] { Esc, EscEsc },
            _ => new[] { value },
        };
    }
}
