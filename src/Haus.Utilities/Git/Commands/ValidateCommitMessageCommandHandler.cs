using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using Haus.Utilities.Common.Cli;
using Microsoft.Extensions.Logging;

namespace Haus.Utilities.Git.Commands;

[Command("git", "validate-commit-message")]
public record ValidateCommitMessageCommand : ICommand;

public class InvalidCommitMessageException(string message) : Exception(message);

public class ValidateCommitMessageCommandHandler(
    IConventionalCommitMessageValidator validator,
    ILogger<ValidateCommitMessageCommandHandler> logger
) : ICommandHandler<ValidateCommitMessageCommand>
{
    public async Task Handle(ValidateCommitMessageCommand request, CancellationToken cancellationToken)
    {
        var commitMessageFilePath = Environment.GetEnvironmentVariable("COMMIT_MSG_FILE");
        if (string.IsNullOrWhiteSpace(commitMessageFilePath))
            throw new InvalidOperationException("COMMIT_MSG_FILE environment variable was not set.");

        var commitMessage = await File.ReadAllTextAsync(commitMessageFilePath, cancellationToken);
        var result = validator.Validate(commitMessage);
        if (!result.IsValid)
            throw new InvalidCommitMessageException(result.Error);

        logger.LogInformation("Commit message is valid.");
    }
}
