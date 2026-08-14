using System;
using System.Text.RegularExpressions;

namespace Haus.Utilities.Release;

public interface ISemVerBumper
{
    string Bump(string currentVersion, string bumpKind);
}

public partial class SemVerBumper : ISemVerBumper
{
    public string Bump(string currentVersion, string bumpKind)
    {
        var (major, minor, patch) = Parse(currentVersion);
        return bumpKind.Trim().ToLowerInvariant() switch
        {
            "major" => $"v{major + 1}.0.0",
            "minor" => $"v{major}.{minor + 1}.0",
            "patch" => $"v{major}.{minor}.{patch + 1}",
            _ => throw new ArgumentException($"Unknown bump kind '{bumpKind}'", nameof(bumpKind)),
        };
    }

    private static (int Major, int Minor, int Patch) Parse(string currentVersion)
    {
        if (string.IsNullOrWhiteSpace(currentVersion))
            return (0, 0, 0);

        var match = VersionPattern().Match(currentVersion.Trim());
        if (!match.Success)
            throw new FormatException($"'{currentVersion}' is not a valid vX.Y.Z version");

        return (
            int.Parse(match.Groups["major"].Value),
            int.Parse(match.Groups["minor"].Value),
            int.Parse(match.Groups["patch"].Value)
        );
    }

    [GeneratedRegex(@"^v?(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)$")]
    private static partial Regex VersionPattern();
}
