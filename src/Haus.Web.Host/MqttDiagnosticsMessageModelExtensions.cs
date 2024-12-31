using Haus.Core.Models;
using Haus.Core.Models.Diagnostics;
using MQTTnet;

namespace Haus.Web.Host;

public static class MqttDiagnosticsMessageModelExtensions
{
    public static MqttApplicationMessage ToMqttMessage(this MqttDiagnosticsMessageModel model)
    {
        return new MqttApplicationMessage
        {
            Topic = model.Topic,
            PayloadSegment = HausJsonSerializer.SerializeToBytes(model.Payload)
        };
    }
}