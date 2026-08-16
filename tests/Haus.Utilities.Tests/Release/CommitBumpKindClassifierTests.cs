using Haus.Utilities.Release;
using Xunit;

namespace Haus.Utilities.Tests.Release;

public class CommitBumpKindClassifierTests
{
    private readonly CommitBumpKindClassifier _classifier = new();

    [Fact]
    public void WhenThereAreNoCommitsThenBumpsPatch()
    {
        var bumpKind = _classifier.Classify([]);

        Assert.Equal("patch", bumpKind);
    }

    [Theory]
    [InlineData("fix: correct zigbee timeout")]
    [InlineData("chore: update gitignore")]
    [InlineData("docs: document release process")]
    public void WhenAllCommitsArePatchLikeThenBumpsPatch(string commitMessage)
    {
        var bumpKind = _classifier.Classify([commitMessage]);

        Assert.Equal("patch", bumpKind);
    }

    [Fact]
    public void WhenAFeatCommitIsPresentThenBumpsMinor()
    {
        var bumpKind = _classifier.Classify(["fix: correct zigbee timeout", "feat: add device discovery"]);

        Assert.Equal("minor", bumpKind);
    }

    [Fact]
    public void WhenACommitHasBreakingChangeMarkerSuffixThenBumpsMajor()
    {
        var bumpKind = _classifier.Classify(["feat(api)!: change device response shape"]);

        Assert.Equal("major", bumpKind);
    }

    [Fact]
    public void WhenACommitHasBreakingChangeFooterThenBumpsMajor()
    {
        var bumpKind = _classifier.Classify([
            "fix: correct zigbee timeout\n\nBREAKING CHANGE: removes the legacy endpoint",
        ]);

        Assert.Equal("major", bumpKind);
    }

    [Fact]
    public void WhenBreakingAndFeatCommitsAreBothPresentThenMajorTakesPrecedence()
    {
        var bumpKind = _classifier.Classify(["feat: add device discovery", "fix(api)!: change response shape"]);

        Assert.Equal("major", bumpKind);
    }

    [Theory]
    [InlineData("Merge branch 'main' into feature/x")]
    [InlineData("added device discovery")]
    [InlineData("")]
    [InlineData("   ")]
    public void WhenCommitMessageIsNotConventionalThenItIsIgnored(string commitMessage)
    {
        var bumpKind = _classifier.Classify([commitMessage]);

        Assert.Equal("patch", bumpKind);
    }

    [Fact]
    public void WhenNonConventionalCommitsAreMixedWithAFeatCommitThenBumpsMinor()
    {
        var bumpKind = _classifier.Classify([
            "Merge branch 'main' into feature/x",
            "feat: add device discovery",
            "added stray notes",
        ]);

        Assert.Equal("minor", bumpKind);
    }
}
