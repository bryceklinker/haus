using System;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Connection;

// Reads the network configuration the deCONZ coordinator already holds, one read-parameter
// round-trip at a time, and assembles it into a NetworkConfig. It never writes a parameter or
// re-forms the network — the coordinator was configured elsewhere and we only read it back.
public class DeconzConnection(DeconzChannel channel)
{
    private const byte MacAddressParameterId = 0x01;
    private const byte PanIdParameterId = 0x05;
    private const byte ChannelParameterId = 0x1C;

    private readonly DeconzChannel _channel = channel;
    private byte _nextSequenceNumber;

    public async Task<NetworkConfig> ConnectAsync(CancellationToken token)
    {
        var macAddress = await ReadMacAddressAsync(token);
        var panId = await ReadPanIdAsync(token);
        var channelNumber = await ReadChannelAsync(token);
        return new NetworkConfig(macAddress, panId, channelNumber);
    }

    private async Task<IeeeAddress> ReadMacAddressAsync(CancellationToken token)
    {
        var value = await ReadParameterAsync(MacAddressParameterId, token);
        return new IeeeAddress(BinaryPrimitives.ReadUInt64LittleEndian(value));
    }

    private async Task<ushort> ReadPanIdAsync(CancellationToken token)
    {
        var value = await ReadParameterAsync(PanIdParameterId, token);
        return BinaryPrimitives.ReadUInt16LittleEndian(value);
    }

    private async Task<byte> ReadChannelAsync(CancellationToken token)
    {
        var value = await ReadParameterAsync(ChannelParameterId, token);
        return value[0];
    }

    private async Task<byte[]> ReadParameterAsync(byte parameterId, CancellationToken token)
    {
        var request = new ReadParameterRequest(_nextSequenceNumber++, parameterId, []);
        var responseFrame = await _channel.SendAndReceiveAsync(ReadParameterFrame.Encode(request), token);
        var response = ReadParameterFrame.Decode(responseFrame);
        if (response is null)
            throw new InvalidOperationException(
                $"Received a truncated read-parameter response for parameter 0x{parameterId:X2}"
            );

        return response.Value;
    }
}
