using System.Text.Json;
using Haus.Core.Models;
using Haus.Core.Models.Discovery;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers;
using Microsoft.Extensions.Options;
using MQTTnet;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Mappers
{
    public class ZigbeeToHausModelMapperTests
    {
        private const string Zigbee2MqttTopic = "zigbee2mqtt";
        private const string HausEventTopic = "haus/events";
        private readonly ZigbeeToHausModelMapper _mapper;

        public ZigbeeToHausModelMapperTests()
        {
            var zigbeeOptions = Options.Create(new ZigbeeOptions
            {
                Config = new Zigbee2MqttConfiguration
                {
                    Mqtt = new MqttConfiguration
                    {
                        BaseTopic = Zigbee2MqttTopic
                    }
                }
            });
            var hausOptions = Options.Create(new HausOptions
            {
                EventsTopic = HausEventTopic
            });
            _mapper = new ZigbeeToHausModelMapper(zigbeeOptions, hausOptions);
        }

        [Fact]
        public void WhenInterviewSuccessfulMessageThenReturnsDeviceDiscovered()
        {
            var message = new MqttApplicationMessage
            {
                Topic = $"{Zigbee2MqttTopic}/bridge/log",
                Payload = Zigbee2MqttMessages.InterviewSuccessful("this-is-an-id")
            };

            var result = _mapper.ToHausEvent(message);

            Assert.Equal(HausEventTopic, result.Topic);
            var hausEvent = JsonSerializer.Deserialize<HausEvent>(result.Payload);
            Assert.Equal(DeviceDiscoveredModel.Type, hausEvent.Type);
            
            var hausEventPayload = hausEvent.GetPayload<DeviceDiscoveredModel>();
            Assert.Equal("this-is-an-id", hausEventPayload.Id);
        }
    }
}