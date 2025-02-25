using Haus.Core.Models.Common;

namespace Haus.Core.Common.Entities;

public record Metadata(string? Key = null, string? Value = null)
{
    public string? Key { get; set; } = Key;
    public string? Value { get; set; } = Value;

    public virtual MetadataModel ToModel()
    {
        return new MetadataModel(Key, Value);
    }

    public static Metadata FromModel(MetadataModel model)
    {
        return new Metadata(model.Key, model.Value);
    }
}
