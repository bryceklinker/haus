using Haus.Utilities.Git;
using Xunit;

namespace Haus.Utilities.Tests.Git;

public class ConventionalCommitMessageValidatorTests
{
    private readonly ConventionalCommitMessageValidator _validator = new();

    [Theory]
    [InlineData("feat: add device discovery")]
    [InlineData("fix: correct zigbee timeout")]
    [InlineData("docs: document commit conventions")]
    [InlineData("style: reformat with csharpier")]
    [InlineData("refactor: extract command factory")]
    [InlineData("perf: reduce allocation in mqtt handler")]
    [InlineData("test: cover empty scope")]
    [InlineData("build: bump dotnet sdk")]
    [InlineData("ci: add release workflow")]
    [InlineData("chore: update gitignore")]
    [InlineData("revert: undo bad migration")]
    public void WhenHeaderUsesAnAllowedTypeWithNoScopeThenIsValid(string message)
    {
        var result = _validator.Validate(message);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void WhenHeaderHasScopeThenIsValid()
    {
        var result = _validator.Validate("fix(zigbee): handle disconnect race");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void WhenHeaderHasBreakingChangeMarkerThenIsValid()
    {
        var result = _validator.Validate("feat(api)!: change device response shape");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void WhenMessageHasBodyAfterHeaderThenOnlyHeaderIsValidated()
    {
        var result = _validator.Validate("feat: add device discovery\n\nSome longer explanation.");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void WhenTypeIsNotAllowedThenIsInvalid()
    {
        var result = _validator.Validate("feature: add device discovery");

        Assert.False(result.IsValid);
    }

    [Fact]
    public void WhenTypeIsUppercaseThenIsInvalid()
    {
        var result = _validator.Validate("Feat: add device discovery");

        Assert.False(result.IsValid);
    }

    [Fact]
    public void WhenColonIsMissingThenIsInvalid()
    {
        var result = _validator.Validate("feat add device discovery");

        Assert.False(result.IsValid);
    }

    [Fact]
    public void WhenDescriptionIsMissingThenIsInvalid()
    {
        var result = _validator.Validate("feat:");

        Assert.False(result.IsValid);
    }

    [Fact]
    public void WhenDescriptionIsOnlyWhitespaceThenIsInvalid()
    {
        var result = _validator.Validate("feat:    ");

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\n\n")]
    public void WhenMessageIsEmptyThenIsInvalid(string message)
    {
        var result = _validator.Validate(message);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void WhenInvalidThenErrorExplainsTheExpectedFormat()
    {
        var result = _validator.Validate("added device discovery");

        Assert.False(result.IsValid);
        Assert.Contains("type(scope): description", result.Error);
        Assert.Contains("feat", result.Error);
    }

    [Fact]
    public void WhenMessageIsGitGeneratedMergeCommitThenIsValid()
    {
        var result = _validator.Validate("Merge branch 'main' into feature/x");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void WhenMessageIsGitGeneratedRevertCommitThenIsValid()
    {
        var result = _validator.Validate("Revert \"feat: add device discovery\"\n\nThis reverts commit abc123.");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void WhenValidThenErrorIsEmpty()
    {
        var result = _validator.Validate("feat: add device discovery");

        Assert.Equal(string.Empty, result.Error);
    }

    [Fact]
    public void WhenMessageHasLeadingBlankLinesThenFirstNonBlankLineIsTheHeader()
    {
        var result = _validator.Validate("\n\nfeat: add device discovery");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void WhenHeaderHasBreakingChangeMarkerWithNoScopeThenIsValid()
    {
        var result = _validator.Validate("feat!: change device response shape");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void WhenScopeHasUppercaseCharactersThenIsInvalid()
    {
        var result = _validator.Validate("feat(API): change device response shape");

        Assert.False(result.IsValid);
    }
}
