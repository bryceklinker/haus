using System;
using System.Text;
using System.Text.Json;
using Haus.Core.Common;
using Haus.Core.Models;
using Haus.Core.Models.Diagnostics;

namespace Haus.Core.Diagnostics.Factories
{
    public interface IMqttDiagnosticsMessageFactory
    {
        MqttDiagnosticsMessageModel Create(string topic, byte[] payload);
    }
    
    public class MqttDiagnosticsMessageFactory : IMqttDiagnosticsMessageFactory
    {
        private readonly IClock _clock;

        public MqttDiagnosticsMessageFactory(IClock clock)
        {
            _clock = clock;
        }

        public MqttDiagnosticsMessageModel Create(string topic, byte[] payload)
        {
            return new MqttDiagnosticsMessageModel
            {
                Id = $"{Guid.NewGuid()}",
                Timestamp = _clock.LocalNow,
                Topic = topic,
                Payload = GetPayloadFromBytes(payload)
            };
        }

        private static object GetPayloadFromBytes(byte[] bytes)
        {
            return HausJsonSerializer.TryDeserialize(bytes, out object payload) 
                ? payload 
                : Encoding.UTF8.GetString(bytes);
        }
    }
}