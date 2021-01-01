using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
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

            File.Exists(_configFilePath).Should().BeTrue();
        }

        [Fact]
        public async Task WhenConfigExistsAndOverwritingIsOnThenConfigIsOverwritten()
        {
            await File.WriteAllTextAsync(_configFilePath, "This is my text");
            var options = CreateZigbeeOptions(_configFilePath, overwriteConfig: true);

            await WriteAsync(options);

            (await File.ReadAllTextAsync(_configFilePath)).Should().NotBe("This is my text");
        }

        [Fact]
        public async Task WhenConfigExistsAndOverwritingIsOffThenConfigIsNotRewritten()
        {
            await File.WriteAllTextAsync(_configFilePath, "This is my text");
            var options = CreateZigbeeOptions(_configFilePath, false);

            await WriteAsync(options);

            (await File.ReadAllTextAsync(_configFilePath)).Should().Be("This is my text");
        }

        [Fact]
        public async Task WhenConfigFileIsInDifferentDirectoryThenConfigFileIsWrittenToDirectory()
        {
            _configFilePath = Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), $"{Guid.NewGuid()}", "config.yaml"));

            var options = CreateZigbeeOptions(_configFilePath);

            await WriteAsync(options);


            File.Exists(_configFilePath).Should().BeTrue();
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