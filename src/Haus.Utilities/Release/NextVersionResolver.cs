using Haus.Utilities.Git;

namespace Haus.Utilities.Release;

public interface INextVersionResolver
{
    string Resolve(string currentVersion, string? versionBumpOverride);
}

public class NextVersionResolver(
    ISemVerBumper bumper,
    ICommitBumpKindClassifier bumpKindClassifier,
    IGitCommitLogReader commitLogReader
) : INextVersionResolver
{
    public string Resolve(string currentVersion, string? versionBumpOverride)
    {
        var versionBump = string.IsNullOrWhiteSpace(versionBumpOverride)
            ? bumpKindClassifier.Classify(commitLogReader.GetCommitMessagesSince(currentVersion))
            : versionBumpOverride;

        return bumper.Bump(currentVersion, versionBump);
    }
}
