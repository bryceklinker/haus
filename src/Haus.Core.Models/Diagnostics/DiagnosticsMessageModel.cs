using System;

namespace Haus.Core.Models.Diagnostics
{
    public class MqttDiagnosticsMessageModel
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Topic { get; set; }
        public object Payload { get; set; }
    }
}