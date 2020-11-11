using System.IO;
using System.Threading.Tasks;
using Haus.Zigbee.Host.Options;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;

namespace Haus.Zigbee.Host.Configuration
{
    public interface IZigbee2MqttConfigurationWriter
    {
        Task WriteConfigAsync();
    }

    public class Zigbee2MqttConfigurationWriter : IZigbee2MqttConfigurationWriter
    {
        private readonly IOptions<ZigbeeOptions> _options;

        public ZigbeeOptions Options => _options.Value;

        public Zigbee2MqttConfigurationWriter(IOptions<ZigbeeOptions> options)
        {
            _options = options;
        }

        public async Task WriteConfigAsync()
        {
            var configFileDirectory = Path.GetDirectoryName(Options.ConfigFile);
            if (!Directory.Exists(configFileDirectory))
            {
                Directory.CreateDirectory(configFileDirectory);
            }
            
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(Options.Config);
            await File.WriteAllTextAsync(Options.ConfigFile, yaml);
        }
    }
}