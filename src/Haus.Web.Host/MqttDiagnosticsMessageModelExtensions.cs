using System.Text.Json;
using Haus.Core.Models.Diagnostics;
using MQTTnet;

namespace Haus.Web.Host
{
    public static class MqttDiagnosticsMessageModelExtensions
    {
        public static MqttApplicationMessage ToMqttMessage(this MqttDiagnosticsMessageModel model)
        {
            return new MqttApplicationMessage
            {
                Topic = model.Topic,
                Payload = JsonSerializer.SerializeToUtf8Bytes(model.Payload)
            };
        }
    }
}