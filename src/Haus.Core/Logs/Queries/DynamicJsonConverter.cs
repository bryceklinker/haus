using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Haus.Core.Logs.Queries;

public class DynamicJsonConverter : JsonConverter<dynamic>
{
    public override dynamic Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:
                return true;
            case JsonTokenType.False:
                return false;
            case JsonTokenType.Number when reader.TryGetInt64(out var l):
                return l;
            case JsonTokenType.Number:
                return reader.GetDouble();
            case JsonTokenType.String when reader.TryGetDateTime(out var datetime):
                return datetime;
            case JsonTokenType.String:
                return reader.GetString() ?? "";
            case JsonTokenType.StartObject:
            {
                using var documentV = JsonDocument.ParseValue(ref reader);
                return ReadObject(documentV.RootElement);
            }
            default:
            {
                var document = JsonDocument.ParseValue(ref reader);
                return document.RootElement.Clone();
            }
        }
    }

    private object ReadObject(JsonElement jsonElement)
    {
        IDictionary<string, object?> expandoObject = new ExpandoObject();
        foreach (var obj in jsonElement.EnumerateObject())
        {
            var k = obj.Name;
            var value = ReadValue(obj.Value);
            expandoObject[k] = value;
        }

        return expandoObject;
    }

    private object? ReadValue(JsonElement jsonElement)
    {
        return jsonElement.ValueKind switch
        {
            JsonValueKind.Object => ReadObject(jsonElement),
            JsonValueKind.Array => ReadList(jsonElement),
            JsonValueKind.String => jsonElement.GetString(),
            JsonValueKind.Number => jsonElement.TryGetInt64(out var l) ? l : 0,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Undefined or JsonValueKind.Null => null,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private object? ReadList(JsonElement jsonElement)
    {
        var list = jsonElement.EnumerateArray().Select(ReadValue).ToList();
        return list.Count == 0 ? null : list;
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        // writer.WriteStringValue(value.ToString());
    }
}
