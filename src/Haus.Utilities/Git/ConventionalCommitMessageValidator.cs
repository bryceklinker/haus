using System;
using System.Text.RegularExpressions;

namespace Haus.Utilities.Git;

public record CommitMessageValidationResult(bool IsValid, string Error)
{
    public static CommitMessageValidationResult Valid() => new(true, string.Empty);

    public static CommitMessageValidationResult Invalid(string error) => new(false, error);
}

public interface IConventionalCommitMessageValidator
{
    CommitMessageValidationResult Validate(string commitMessage);
}

public readonly record struct ConventionalCommitHeader(string Type, bool IsBreakingChange);

// Shared with CommitBumpKindClassifier so both the commit-msg hook and the release
// bump-kind detection agree on what a conventional commit header looks like.
public static class ConventionalCommitHeaderParser
{
    public static readonly string[] AllowedTypes =
    [
        "feat",
        "fix",
        "docs",
        "style",
        "refactor",
        "perf",
        "test",
        "build",
        "ci",
        "chore",
        "revert",
    ];

    private static readonly Regex HeaderPattern = new(
        $"^(?<type>{string.Join('|', AllowedTypes)})(\\([a-z0-9/_-]+\\))?(?<breaking>!)?: \\S.*$",
        RegexOptions.Compiled
    );

    public static bool TryParse(string header, out ConventionalCommitHeader parsed)
    {
        var match = HeaderPattern.Match(header);
        if (!match.Success)
        {
            parsed = default;
            return false;
        }

        parsed = new ConventionalCommitHeader(match.Groups["type"].Value, match.Groups["breaking"].Success);
        return true;
    }

    public static string FirstLine(string commitMessage)
    {
        foreach (var line in commitMessage.Split('\n'))
        {
            var trimmed = line.Trim();
            if (trimmed.Length > 0)
                return trimmed;
        }

        return string.Empty;
    }
}

public class ConventionalCommitMessageValidator : IConventionalCommitMessageValidator
{
    public CommitMessageValidationResult Validate(string commitMessage)
    {
        var header = ConventionalCommitHeaderParser.FirstLine(commitMessage);

        if (IsGitGeneratedHeader(header))
            return CommitMessageValidationResult.Valid();

        return ConventionalCommitHeaderParser.TryParse(header, out _)
            ? CommitMessageValidationResult.Valid()
            : CommitMessageValidationResult.Invalid(BuildError(header));
    }

    private static bool IsGitGeneratedHeader(string header) =>
        header.StartsWith("Merge ", StringComparison.Ordinal)
        || header.StartsWith("Revert \"", StringComparison.Ordinal);

    private static string BuildError(string header) =>
        $"""
            Invalid commit message: "{header}"

            Commit messages must follow Conventional Commits format:
              type(scope): description

            Allowed types: {string.Join(", ", ConventionalCommitHeaderParser.AllowedTypes)}

            Examples:
              feat: add device discovery
              fix(zigbee): handle disconnect race
              feat(api)!: change device response shape

            See CONTRIBUTING.md for details.
            """;
}
