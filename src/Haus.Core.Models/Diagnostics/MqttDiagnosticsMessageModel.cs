using System;

namespace Haus.Core.Models.Diagnostics;

public record MqttDiagnosticsMessageModel
{
    public string Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Topic { get; set; }
    public object Payload { get; set; }
}