using System;

namespace Haus.Core.Models.ExternalMessages;

[SkipGeneration]
public record HausEvent(string? Type = null, string? Timestamp = null)
{
    public string Timestamp { get; init; } = Timestamp ?? DateTime.Now.ToString("O");
}

[SkipGeneration]
public record HausEvent<T>(string Type, T Payload, string? Timestamp = null) : HausEvent(Type, Timestamp);
