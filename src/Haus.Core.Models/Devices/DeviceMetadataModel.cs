namespace Haus.Core.Models.Devices
{
    public class DeviceMetadataModel
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public DeviceMetadataModel(string key = null, string value = null)
        {
            Key = key;
            Value = value;
        }
    }
}