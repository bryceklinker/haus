using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using Haus.Utilities.Common.Cli;
using Microsoft.Extensions.Logging;

namespace Haus.Utilities.Packaging.Commands;

[Command("packaging", "render-deb-compose")]
public record RenderDebComposeCommand : ICommand;

public class RenderDebComposeCommandHandler(
    IDebComposeVersionPinner pinner,
    ILogger<RenderDebComposeCommandHandler> logger
) : ICommandHandler<RenderDebComposeCommand>
{
    // Resolved against the process's current working directory: the build
    // script cd's into the staged packaging/deb tree before invoking this
    // command, so these paths land on etc/haus/docker-compose.yml(.template)
    // within that staged copy, not the checked-out source template.
    private static readonly string TemplatePath = Path.Combine("etc", "haus", "docker-compose.yml.template");
    private static readonly string OutputPath = Path.Combine("etc", "haus", "docker-compose.yml");

    public async Task Handle(RenderDebComposeCommand request, CancellationToken cancellationToken)
    {
        var version = Environment.GetEnvironmentVariable("VERSION");
        logger.LogInformation("Rendering {OutputPath} for version {Version}", OutputPath, version);

        var template = await File.ReadAllTextAsync(TemplatePath, cancellationToken);
        var pinned = pinner.Pin(template, version ?? string.Empty);
        await File.WriteAllTextAsync(OutputPath, pinned, cancellationToken);
    }
}
