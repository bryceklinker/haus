using System;
using System.Text.RegularExpressions;

namespace Haus.Utilities.Git;

public record CommitMessageValidationResult(bool IsValid, string? Error)
{
    public static CommitMessageValidationResult Valid() => new(true, null);

    public static CommitMessageValidationResult Invalid(string error) => new(false, error);
}

public interface IConventionalCommitMessageValidator
{
    CommitMessageValidationResult Validate(string commitMessage);
}

public class ConventionalCommitMessageValidator : IConventionalCommitMessageValidator
{
    private static readonly string[] AllowedTypes =
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
        $"^({string.Join('|', AllowedTypes)})(\\([a-z0-9/_-]+\\))?!?: \\S.*$",
        RegexOptions.Compiled
    );

    public CommitMessageValidationResult Validate(string commitMessage)
    {
        var header = FirstLine(commitMessage);

        if (IsGitGeneratedHeader(header))
            return CommitMessageValidationResult.Valid();

        return HeaderPattern.IsMatch(header)
            ? CommitMessageValidationResult.Valid()
            : CommitMessageValidationResult.Invalid(BuildError(header));
    }

    private static string FirstLine(string commitMessage)
    {
        var newlineIndex = commitMessage.IndexOf('\n');
        var header = newlineIndex >= 0 ? commitMessage[..newlineIndex] : commitMessage;
        return header.Trim();
    }

    private static bool IsGitGeneratedHeader(string header) =>
        header.StartsWith("Merge ", StringComparison.Ordinal)
        || header.StartsWith("Revert \"", StringComparison.Ordinal);

    private static string BuildError(string header) =>
        $"""
            Invalid commit message: "{header}"

            Commit messages must follow Conventional Commits format:
              type(scope): description

            Allowed types: {string.Join(", ", AllowedTypes)}

            Examples:
              feat: add device discovery
              fix(zigbee): handle disconnect race
              feat(api)!: change device response shape

            See CONTRIBUTING.md for details.
            """;
}
