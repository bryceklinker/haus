using System;
using System.Text;
using FluentAssertions;
using Haus.Core.Diagnostics.Factories;
using Haus.Core.Models;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Newtonsoft.Json.Linq;
using Xunit;

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
        public void WhenCreatedWithNullBytesThenReturnsNullPayload()
        {
            var model = _factory.Create("something", null);

            model.Topic.Should().Be("something");
            model.Payload.Should().BeNull();
        }

        [Fact]
        public void WhenCreatedWithEmptyPayloadThenReturnsEmptyPayload()
        {
            var model = _factory.Create("something", Array.Empty<byte>());

            model.Topic.Should().Be("something");
            model.Payload.Should().Be("");
        }
        
        [Fact]
        public void WhenCreatedThenTopicAndPayloadArePopulated()
        {
            var model = _factory.Create("my-topic", HausJsonSerializer.SerializeToBytes(new {id = 45}));

            model.Topic.Should().Be("my-topic");
            JObject.Parse(model.Payload.ToString()).Value<int>("id").Should().Be(45);
        }

        [Fact]
        public void WhenCreatedWithSingleValuePayloadThenPayloadIsString()
        {
            var model = _factory.Create("my-topic", Encoding.UTF8.GetBytes("hello there"));

            model.Payload.Should().Be("hello there");
        }

        [Fact]
        public void WhenCreatedThenIdIsAssignedToMessage()
        {
            var model = _factory.Create("idk", Array.Empty<byte>());

            model.Id.Should().BeAGuid();
        }

        [Fact]
        public void WhenCreatedThenTimestampIsAssignedToMessage()
        {
            _clock.SetNow(new DateTime(2020, 9, 23));
            var model = _factory.Create("idk", Array.Empty<byte>());

            model.Timestamp.Should().Be(new DateTime(2020, 9, 23));
        }
    }
}