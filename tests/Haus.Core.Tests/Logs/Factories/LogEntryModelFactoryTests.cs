using FluentAssertions;
using Haus.Core.Logs.Factories;
using Haus.Core.Models;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Haus.Core.Tests.Logs.Factories;

public class LogEntryModelFactoryTests
{
    private const string StandardLine =
        @"{""@t"":""2021-01-17T15:27:12.3639990Z"",""@m"":""RX (413 bytes) <<< \""Publish: [Topic=haus/commands] [Payload.Length=396] [QoSLevel=AtMostOnce] [Dup=False] [Retain=False] [PacketIdentifier=]\"""",""@i"":""fb9eec8d"",""@l"":""Debug"",""0"":413,""1"":""Publish: [Topic=haus/commands] [Payload.Length=396] [QoSLevel=AtMostOnce] [Dup=False] [Retain=False] [PacketIdentifier=]"",""SourceContext"":""Haus.Mqtt.Client.Logging.MqttLogger"",""Application"":""Haus Web""}";

    private const string MissingLevelLine =
        @"{""@t"":""2021-01-17T15:27:12.3515550Z"",""@m"":""Enqueuing domain event RoomLightingChangedDomainEvent..."",""@i"":""fc362093"",""SourceContext"":""Haus.Cqrs.DomainEvents.LoggingDomainEventBus"",""ActionId"":""e27aedd7-9bf7-4069-9d3c-ce458cd24545"",""ActionName"":""Haus.Web.Host.Rooms.RoomsController.ChangeLighting (Haus.Web.Host)"",""RequestId"":""0HM5QVHBISRUP:00000007"",""RequestPath"":""/api/rooms/1/lighting"",""ConnectionId"":""0HM5QVHBISRUP"",""Application"":""Haus Web""}";

    private readonly LogEntryModelFactory _factory = new();

    [Fact]
    public void WhenLogLineProvidedThenCreatesLogEntryFromLine()
    {
        var entry = _factory.CreateFromLine(StandardLine);

        entry.Timestamp.Should().Be("2021-01-17T15:27:12.3639990Z");
        entry.Level.Should().Be("Debug");
        entry.Message.Should()
            .Be(
                "RX (413 bytes) <<< \"Publish: [Topic=haus/commands] [Payload.Length=396] [QoSLevel=AtMostOnce] [Dup=False] [Retain=False] [PacketIdentifier=]\"");
    }

    [Fact]
    public void WhenLogLineProvidedReturnsPropertiesFromLine()
    {
        var entry = _factory.CreateFromLine(StandardLine);

        var json = HausJsonSerializer.Serialize(entry.Value);
        var jObject = JObject.Parse(json);

        jObject.Value<string>("@m").Should()
            .Be(
                "RX (413 bytes) <<< \"Publish: [Topic=haus/commands] [Payload.Length=396] [QoSLevel=AtMostOnce] [Dup=False] [Retain=False] [PacketIdentifier=]\"");
    }

    [Fact]
    public void WhenLogLineIsMissingLevelThenReturnsInformationLevel()
    {
        var entry = _factory.CreateFromLine(MissingLevelLine);

        entry.Level.Should().Be("Information");
    }
}