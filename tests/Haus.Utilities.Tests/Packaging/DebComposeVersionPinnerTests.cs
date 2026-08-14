using System;
using Haus.Utilities.Packaging;
using Xunit;

namespace Haus.Utilities.Tests.Packaging;

public class DebComposeVersionPinnerTests
{
    private readonly DebComposeVersionPinner _pinner = new();

    [Fact]
    public void WhenTemplateHasHausWebLatestThenPinsToGivenVersion()
    {
        var pinned = _pinner.Pin("image: bryceklinker/personal:haus-web-latest", "1.2.3");

        Assert.Equal("image: bryceklinker/personal:haus-web-latest".Replace("latest", "1.2.3"), pinned);
    }

    [Fact]
    public void WhenTemplateHasAllThreeHausServicesThenPinsEachToGivenVersion()
    {
        const string template = """
            haus_zigbee:
              image: bryceklinker/personal:haus-zigbee-latest
            haus_web:
              image: bryceklinker/personal:haus-web-latest
            haus_sit:
              image: bryceklinker/personal:haus-site-latest
            """;

        var pinned = _pinner.Pin(template, "2.0.0");

        Assert.Contains("bryceklinker/personal:haus-zigbee-2.0.0", pinned);
        Assert.Contains("bryceklinker/personal:haus-web-2.0.0", pinned);
        Assert.Contains("bryceklinker/personal:haus-site-2.0.0", pinned);
    }

    [Fact]
    public void WhenTemplateHasUnrelatedLatestImageThenLeavesItUnchanged()
    {
        var pinned = _pinner.Pin("image: eclipse-mosquitto:latest", "1.2.3");

        Assert.Equal("image: eclipse-mosquitto:latest", pinned);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void WhenVersionIsEmptyThenThrows(string? version)
    {
        Assert.Throws<InvalidOperationException>(() =>
            _pinner.Pin("image: bryceklinker/personal:haus-web-latest", version!)
        );
    }
}
