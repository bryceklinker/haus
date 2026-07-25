using System;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Connection;

// Reads the network configuration the deCONZ coordinator already holds, one read-parameter
// round-trip at a time, and assembles it into a NetworkConfig. It never writes a parameter or
// re-forms the network — the coordinator was configured elsewhere and we only read it back.
public sealed class DeconzConnection
{
    private const byte MacAddressParameterId = 0x01;
    private const byte PanIdParameterId = 0x05;
    private const byte ChannelParameterId = 0x1C;

    private readonly DeconzChannel _channel;
    private byte _nextSequenceNumber;

    public DeconzConnection(DeconzChannel channel)
    {
        _channel = channel;
    }

    public async Task<NetworkConfig> ConnectAsync(CancellationToken token)
    {
        var macAddress = new IeeeAddress(
            BinaryPrimitives.ReadUInt64LittleEndian(await ReadParameterAsync(MacAddressParameterId, token))
        );
        var panId = BinaryPrimitives.ReadUInt16LittleEndian(await ReadParameterAsync(PanIdParameterId, token));
        var channel = (await ReadParameterAsync(ChannelParameterId, token))[0];
        return new NetworkConfig(macAddress, panId, channel);
    }

    private async Task<byte[]> ReadParameterAsync(byte parameterId, CancellationToken token)
    {
        var request = new ReadParameterRequest(_nextSequenceNumber++, parameterId, Array.Empty<byte>());
        var responseFrame = await _channel.SendAndReceiveAsync(ReadParameterFrame.Encode(request), token);
        return ReadParameterFrame.Decode(responseFrame).Value;
    }
}
