using System;

namespace Haus.Core.Models.ExternalMessages;

[SkipGeneration]
public record HausEvent(string? Type = null)
{
    public string Timestamp { get; } = DateTime.Now.ToString("O");
}

[SkipGeneration]
public record HausEvent<T>(string Type, T Payload) : HausEvent(Type);
