using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Haus.Zigbee.Tests.Transport;

public class FakeSerialTransportTests
{
    private readonly FakeSerialTransport _transport = new();

    [Fact]
    public async Task WhenBytesAreWrittenThenTheyAreCapturedInWriteOrder()
    {
        await _transport.WriteAsync(new byte[] { 0x01, 0x02 }, CancellationToken.None);
        await _transport.WriteAsync(new byte[] { 0x03 }, CancellationToken.None);

        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, _transport.WrittenBytes);
    }

    [Fact]
    public async Task WhenIncomingBytesAreFedThenReadReturnsThemIntoTheBufferAndReportsTheCount()
    {
        _transport.FeedIncoming(new byte[] { 0x0a, 0x0b, 0x0c });

        var buffer = new byte[8];
        var count = await _transport.ReadAsync(buffer, CancellationToken.None);

        Assert.Equal(3, count);
        Assert.Equal(new byte[] { 0x0a, 0x0b, 0x0c }, buffer[..count]);
    }

    [Fact]
    public async Task WhenBufferIsSmallerThanIncomingThenReadFillsBufferAndLeavesRemainderForNextRead()
    {
        _transport.FeedIncoming(new byte[] { 0x01, 0x02, 0x03 });

        var firstBuffer = new byte[2];
        var firstCount = await _transport.ReadAsync(firstBuffer, CancellationToken.None);

        var secondBuffer = new byte[2];
        var secondCount = await _transport.ReadAsync(secondBuffer, CancellationToken.None);

        Assert.Equal(new byte[] { 0x01, 0x02 }, firstBuffer[..firstCount]);
        Assert.Equal(new byte[] { 0x03 }, secondBuffer[..secondCount]);
    }
}
