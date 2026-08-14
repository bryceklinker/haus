using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using Haus.Utilities.Common.Cli;
using Microsoft.Extensions.Logging;

namespace Haus.Utilities.Release.Commands;

[Command("release", "bump-version")]
public record BumpVersionCommand : ICommand;

public class BumpVersionCommandHandler(ISemVerBumper bumper, ILogger<BumpVersionCommandHandler> logger)
    : ICommandHandler<BumpVersionCommand>
{
    // Logging goes to the console (see HausLogger.ConfigureConsoleOnly), so the
    // computed version is written to a file rather than stdout -- scripts that
    // capture "the next version" would otherwise have to filter log lines out
    // of their own output.
    private static readonly string OutputPath =
        Environment.GetEnvironmentVariable("NEXT_VERSION_FILE") ?? "next-version.txt";

    public async Task Handle(BumpVersionCommand request, CancellationToken cancellationToken)
    {
        var currentVersion = Environment.GetEnvironmentVariable("CURRENT_VERSION") ?? string.Empty;
        var versionBump = Environment.GetEnvironmentVariable("VERSION_BUMP") ?? "patch";

        var nextVersion = bumper.Bump(currentVersion, versionBump);
        logger.LogInformation(
            "Bumping {CurrentVersion} ({BumpKind}) -> {NextVersion}",
            currentVersion,
            versionBump,
            nextVersion
        );

        await File.WriteAllTextAsync(OutputPath, nextVersion, cancellationToken);
    }
}
