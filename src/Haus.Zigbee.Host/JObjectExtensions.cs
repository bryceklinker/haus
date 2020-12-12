using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models;
using Haus.Core.Models.Devices;
using Newtonsoft.Json.Linq;

namespace Haus.Zigbee.Host
{
    public static class JObjectExtensions
    {
        private static readonly string[] KnownMetadata =
        {
            "friendly_name"
        };
        public static IEnumerable<DeviceMetadataModel> ToDeviceMetadata(this JObject jObject)
        {
            return jObject.Properties()
                .Where(prop => KnownMetadata.Missing(prop.Name))
                .Select(prop => new DeviceMetadataModel
                {
                    Key = prop.Name,
                    Value = prop.Value.ToString()
                });
        }
    }
}