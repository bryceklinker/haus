using System.Collections.Generic;

namespace Haus.Zigbee.Serial;

// SLIP framing (RFC 1055): END delimits frames in the incoming stream. This decoder is
// stateful so it can accumulate a frame across partial reads and yield each frame once its
// terminating END arrives; the accumulator is the one place mutation is deliberately kept.
public class SlipDecoder
{
    private const byte End = 0xC0;
    private const byte Esc = 0xDB;
    private const byte EscEnd = 0xDC;
    private const byte EscEsc = 0xDD;

    private readonly List<byte> _currentFrame = new();
    private bool _isEscaped;

    public IReadOnlyList<byte[]> Decode(byte[] chunk)
    {
        var frames = new List<byte[]>();
        foreach (var value in chunk)
            Consume(value, frames);
        return frames;
    }

    private void Consume(byte value, List<byte[]> frames)
    {
        if (_isEscaped)
        {
            _currentFrame.Add(Unescape(value));
            _isEscaped = false;
            return;
        }
        if (value == End)
        {
            EndFrame(frames);
            return;
        }
        if (value == Esc)
        {
            _isEscaped = true;
            return;
        }
        _currentFrame.Add(value);
    }

    private void EndFrame(List<byte[]> frames)
    {
        if (_currentFrame.Count > 0)
            frames.Add(_currentFrame.ToArray());
        _currentFrame.Clear();
    }

    private static byte Unescape(byte value)
    {
        return value switch
        {
            EscEnd => End,
            EscEsc => Esc,
            _ => value,
        };
    }
}
