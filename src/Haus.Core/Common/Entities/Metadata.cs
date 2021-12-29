using Haus.Core.Models.Common;

namespace Haus.Core.Common.Entities;

public record Metadata
{
    public string Key { get; set; }
    public string Value { get; set; }

    public Metadata(string key = null, string value = null)
    {
        Key = key;
        Value = value;
    }

    public virtual MetadataModel ToModel()
    {
        return new MetadataModel(Key, Value);
    }

    public static Metadata FromModel(MetadataModel model)
    {
        return new Metadata(model.Key, model.Value);
    }
}