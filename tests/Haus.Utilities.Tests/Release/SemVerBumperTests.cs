using System;
using Haus.Utilities.Release;
using Xunit;

namespace Haus.Utilities.Tests.Release;

public class SemVerBumperTests
{
    private readonly SemVerBumper _bumper = new();

    [Theory]
    [InlineData("v0.9.10", "patch", "v0.9.11")]
    [InlineData("v0.9.10", "minor", "v0.10.0")]
    [InlineData("v0.9.10", "major", "v1.0.0")]
    [InlineData("v1.2.3", "PATCH", "v1.2.4")]
    [InlineData("0.9.10", "patch", "v0.9.11")]
    public void WhenCurrentVersionIsValidThenBumpsAccordingToKind(
        string currentVersion,
        string bumpKind,
        string expected
    )
    {
        var next = _bumper.Bump(currentVersion, bumpKind);

        Assert.Equal(expected, next);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void WhenCurrentVersionIsMissingThenBumpsFromZero(string? currentVersion)
    {
        var next = _bumper.Bump(currentVersion!, "patch");

        Assert.Equal("v0.0.1", next);
    }

    [Fact]
    public void WhenBumpKindIsUnknownThenThrows()
    {
        Assert.Throws<ArgumentException>(() => _bumper.Bump("v0.9.10", "sideways"));
    }

    [Fact]
    public void WhenCurrentVersionIsMalformedThenThrows()
    {
        Assert.Throws<FormatException>(() => _bumper.Bump("v1.2", "patch"));
    }
}
