using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Haus.Core.Models;

public static class HausJsonSerializer
{
    public static readonly JsonSerializerOptions DefaultOptions = new(JsonSerializerDefaults.Web)
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public static bool TryDeserialize<T>(ArraySegment<byte> bytes, out T? value)
    {
        try
        {
            value = Deserialize<T>(bytes);
            return true;
        }
        catch (JsonException)
        {
            value = default;
            return false;
        }
    }

    public static T? Deserialize<T>(ArraySegment<byte> bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes, DefaultOptions);
    }

    public static T? Deserialize<T>(string json, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
    }

    public static string Serialize(object value, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(value, options ?? DefaultOptions);
    }

    public static ArraySegment<byte> SerializeToBytes(object value)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value, DefaultOptions);
    }
}