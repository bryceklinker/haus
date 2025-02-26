using Haus.Core.Common.Entities;

namespace Haus.Core.Devices.Entities;

public record DeviceMetadataEntity(string Key = "", string Value = "") : Metadata(Key, Value)
{
    public long Id { get; set; }
    public DeviceEntity Device { get; set; }

    public void Update(string value)
    {
        Value = value;
    }
}
