using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Haus.Utilities.Git;

public interface IGitCommitLogReader
{
    IReadOnlyList<string> GetCommitMessagesSince(string? tag);
}

public class GitCommitLogReader : IGitCommitLogReader
{
    // %x1e (record separator) can't appear in a commit message body, so it's safe
    // to split on -- unlike a newline, which commit messages routinely contain.
    private const char CommitSeparator = '\x1e';

    public IReadOnlyList<string> GetCommitMessagesSince(string? tag)
    {
        var range = string.IsNullOrWhiteSpace(tag) ? "HEAD" : $"{tag}..HEAD";
        var output = RunGitLog(range);

        return output
            .Split(CommitSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(message => message.Trim())
            .Where(message => message.Length > 0)
            .ToArray();
    }

    private static string RunGitLog(string range)
    {
        var startInfo = new ProcessStartInfo("git")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        startInfo.ArgumentList.Add("log");
        startInfo.ArgumentList.Add(range);
        startInfo.ArgumentList.Add($"--pretty=format:%B{CommitSeparator}");

        using var process =
            Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start 'git log' process.");
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"'git log {range}' failed: {error}");

        return output;
    }
}
