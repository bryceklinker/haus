using System;
using System.Text;
using Haus.Core.Common;
using Haus.Core.Models;
using Haus.Core.Models.Diagnostics;

namespace Haus.Core.Diagnostics.Factories;

public interface IMqttDiagnosticsMessageFactory
{
    MqttDiagnosticsMessageModel Create(string topic, ArraySegment<byte> payload);
}

public class MqttDiagnosticsMessageFactory(IClock clock) : IMqttDiagnosticsMessageFactory
{
    public MqttDiagnosticsMessageModel Create(string topic, ArraySegment<byte> payload)
    {
        return new MqttDiagnosticsMessageModel
        {
            Id = $"{Guid.NewGuid()}",
            Timestamp = clock.LocalNow,
            Topic = topic,
            Payload = GetPayloadFromBytes(payload),
        };
    }

    private static object GetPayloadFromBytes(ArraySegment<byte> bytes)
    {
        if (bytes == null)
            return null;

        return HausJsonSerializer.TryDeserialize(bytes, out object payload) ? payload : Encoding.UTF8.GetString(bytes);
    }
}
