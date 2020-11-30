using System;
using System.IO;
using System.Threading.Tasks;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Microsoft.Extensions.Options;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Configuration
{
    public class Zigbee2MqttConfigurationWriterTests : IDisposable
    {
        private string _configFilePath;
        
        public Zigbee2MqttConfigurationWriterTests()
        {
            _configFilePath = Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), "config.yaml"));
        }
        
        [Fact]
        public async Task WhenConfigWrittenThenOptionsIsWrittenToConfigFileInYaml()
        {
            var options = CreateZigbeeOptions(_configFilePath);

            await WriteAsync(options);

            Assert.True(File.Exists(_configFilePath));
        }

        [Fact]
        public async Task WhenConfigExistsAndOverwritingIsOnThenConfigIsOverwritten()
        {
            await File.WriteAllTextAsync(_configFilePath, "This is my text");
            var options = CreateZigbeeOptions(_configFilePath, overwriteConfig: true);

            await WriteAsync(options);

            Assert.NotEqual("This is my text", await File.ReadAllTextAsync(_configFilePath));
        }

        [Fact]
        public async Task WhenConfigExistsAndOverwritingIsOffThenConfigIsNotRewritten()
        {
            await File.WriteAllTextAsync(_configFilePath, "This is my text");
            var options = CreateZigbeeOptions(_configFilePath, overwriteConfig: false);

            await WriteAsync(options);

            Assert.Equal("This is my text", await File.ReadAllTextAsync(_configFilePath));
        }

        [Fact]
        public async Task WhenConfigFileIsInDifferentDirectoryThenConfigFileIsWrittenToDirctory()
        {
            _configFilePath = Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), $"{Guid.NewGuid()}", "config.yaml"));

            var options = CreateZigbeeOptions(_configFilePath);

            await WriteAsync(options);

            Assert.True(File.Exists(_configFilePath));
        }
        
        public void Dispose()
        {
            if (File.Exists(_configFilePath))
            {
                File.Delete(_configFilePath);
            }
        }

        private static async Task WriteAsync(ZigbeeOptions zigbeeOptions)
        {
            var options = Options.Create(zigbeeOptions);
            var writer = new Zigbee2MqttConfigurationWriter(options);
            await writer.WriteConfigAsync();
        }

        private static ZigbeeOptions CreateZigbeeOptions(string configFilePath, bool overwriteConfig = true, bool permitJoin = false)
        {
            return new ZigbeeOptions
            {
                OverwriteConfig = overwriteConfig,
                ConfigFile = configFilePath,
                Config = new Zigbee2MqttConfiguration
                {
                    PermitJoin = permitJoin
                }
            };
        }
    }
}