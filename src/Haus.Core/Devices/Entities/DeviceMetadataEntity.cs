using Haus.Core.Common.Entities;

namespace Haus.Core.Devices.Entities
{
    public record DeviceMetadataEntity : Metadata
    {
        public long Id { get; set; }
        public DeviceEntity Device { get; set; }
        
        public DeviceMetadataEntity(string key = null, string value = null)
            : base(key, value)
        {
        }

        public void Update(string value)
        {
            Value = value;
        }
    }
}