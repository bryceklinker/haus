using System;

namespace Haus.Core.Models.Diagnostics;

public record MqttDiagnosticsMessageModel(string? Id, DateTime Timestamp, string? Topic, object? Payload);
