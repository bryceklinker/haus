using System;
using System.Collections.Generic;
using System.Linq;
using Haus.Utilities.Git;

namespace Haus.Utilities.Release;

public interface ICommitBumpKindClassifier
{
    string Classify(IEnumerable<string> commitMessages);
}

public class CommitBumpKindClassifier : ICommitBumpKindClassifier
{
    private const string MajorBumpKind = "major";
    private const string MinorBumpKind = "minor";
    private const string PatchBumpKind = "patch";
    private const string FeatCommitType = "feat";
    private const string BreakingChangeFooterMarker = "BREAKING CHANGE:";

    public string Classify(IEnumerable<string> commitMessages)
    {
        var hasBreakingChange = false;
        var hasFeature = false;

        foreach (var message in commitMessages)
        {
            if (string.IsNullOrWhiteSpace(message))
                continue;

            if (HasBreakingChangeFooter(message))
            {
                hasBreakingChange = true;
                continue;
            }

            var header = ConventionalCommitHeaderParser.FirstLine(message);
            if (!ConventionalCommitHeaderParser.TryParse(header, out var parsed))
                continue;

            if (parsed.IsBreakingChange)
                hasBreakingChange = true;
            else if (parsed.Type == FeatCommitType)
                hasFeature = true;
        }

        if (hasBreakingChange)
            return MajorBumpKind;

        return hasFeature ? MinorBumpKind : PatchBumpKind;
    }

    private static bool HasBreakingChangeFooter(string commitMessage) =>
        commitMessage
            .Split('\n')
            .Any(line => line.TrimStart().StartsWith(BreakingChangeFooterMarker, StringComparison.Ordinal));
}
