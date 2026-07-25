using System.Collections.Generic;

namespace Haus.Zigbee.Serial;

// SLIP framing (RFC 1055): END delimits frames in the incoming stream. This decoder is
// stateful so it can accumulate a frame across partial reads and yield each frame once its
// terminating END arrives; the accumulator is the one place mutation is deliberately kept.
public class SlipDecoder
{
    private const byte End = 0xC0;

    private readonly List<byte> _currentFrame = new();

    public IReadOnlyList<byte[]> Decode(byte[] chunk)
    {
        var frames = new List<byte[]>();
        foreach (var value in chunk)
        {
            if (value == End)
            {
                if (_currentFrame.Count > 0)
                    frames.Add(_currentFrame.ToArray());
                _currentFrame.Clear();
                continue;
            }
            _currentFrame.Add(value);
        }
        return frames;
    }
}
