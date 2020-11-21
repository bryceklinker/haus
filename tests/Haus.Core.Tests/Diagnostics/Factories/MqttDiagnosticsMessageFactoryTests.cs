using System;
using System.Text;
using Haus.Core.Diagnostics.Factories;
using Haus.Testing.Support.Fakes;
using Newtonsoft.Json.Linq;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Haus.Core.Tests.Diagnostics.Factories
{
    public class MqttDiagnosticsMessageFactoryTests
    {
        private readonly FakeClock _clock;
        private readonly MqttDiagnosticsMessageFactory _factory;

        public MqttDiagnosticsMessageFactoryTests()
        {
            _clock = new FakeClock();
            _factory = new MqttDiagnosticsMessageFactory(_clock);
        }

        [Fact]
        public void WhenCreatedThenTopicAndPayloadArePopulated()
        {
            var model = _factory.Create("my-topic", JsonSerializer.SerializeToUtf8Bytes(new {id = 45}));

            Assert.Equal("my-topic", model.Topic);
            Assert.Equal(45, JObject.Parse(model.Payload.ToString()).Value<int>("id"));
        }

        [Fact]
        public void WhenCreatedWithSingleValuePayloadThenPayloadIsString()
        {
            var model = _factory.Create("my-topic", Encoding.UTF8.GetBytes("hello there"));

            Assert.Equal("hello there", model.Payload);
        }

        [Fact]
        public void WhenCreatedThenIdIsAssignedToMessage()
        {
            var model = _factory.Create("idk", Array.Empty<byte>());

            Assert.True(Guid.TryParse(model.Id, out _));
        }

        [Fact]
        public void WhenCreatedThenTimestampIsAssignedToMessage()
        {
            _clock.SetNow(new DateTime(2020, 9, 23));
            var model = _factory.Create("idk", Array.Empty<byte>());

            Assert.Equal(new DateTime(2020, 9, 23), model.Timestamp);
        }
    }
}