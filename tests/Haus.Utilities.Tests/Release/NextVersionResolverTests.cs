using System.Collections.Generic;
using Haus.Utilities.Git;
using Haus.Utilities.Release;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Haus.Utilities.Tests.Release;

public class NextVersionResolverTests
{
    [Fact]
    public void WhenVersionBumpOverrideIsSuppliedThenItTakesPrecedenceOverClassification()
    {
        var resolver = BuildResolver(["feat: add device discovery"]);

        var nextVersion = resolver.Resolve("v1.0.2", "major");

        Assert.Equal("v2.0.0", nextVersion);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void WhenVersionBumpOverrideIsMissingThenBumpKindIsClassifiedFromCommitLog(string? versionBumpOverride)
    {
        var resolver = BuildResolver(["fix: correct zigbee timeout", "feat: add device discovery"]);

        var nextVersion = resolver.Resolve("v1.0.2", versionBumpOverride);

        Assert.Equal("v1.1.0", nextVersion);
    }

    [Fact]
    public void WhenNoOverrideAndNoQualifyingCommitsThenDelegatesPatchBumpToSemVerBumper()
    {
        var resolver = BuildResolver(["fix: correct zigbee timeout"]);

        var nextVersion = resolver.Resolve("v1.0.2", null);

        Assert.Equal("v1.0.3", nextVersion);
    }

    private static INextVersionResolver BuildResolver(IReadOnlyList<string> commitMessages)
    {
        var services = new ServiceCollection().AddHausUtilities();
        services.RemoveAll<IGitCommitLogReader>();
        services.AddSingleton<IGitCommitLogReader>(new StubGitCommitLogReader(commitMessages));

        return services.BuildServiceProvider().GetRequiredService<INextVersionResolver>();
    }

    private class StubGitCommitLogReader(IReadOnlyList<string> commitMessages) : IGitCommitLogReader
    {
        public IReadOnlyList<string> GetCommitMessagesSince(string? tag) => commitMessages;
    }
}
