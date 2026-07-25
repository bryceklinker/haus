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
}
