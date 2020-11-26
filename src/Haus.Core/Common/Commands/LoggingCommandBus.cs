using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Haus.Core.Common.Commands
{
    internal class LoggingCommandBus : LoggingBus, ICommandBus
    {
        private readonly ICommandBus _commandBus;

        protected override string BeginMessage => "Executing command";
        protected override string FinishedMessage => "Finished executing command";
        protected override string ErrorMessage => "Error executing command";

        public LoggingCommandBus(ICommandBus commandBus, ILogger<LoggingCommandBus> logger)
            : base(logger)
        {
            _commandBus = commandBus;
        }

        public async Task ExecuteAsync(ICommand command, CancellationToken token = default)
        {
            await ExecuteWithLoggingAsync(command, async () =>
            {
                await _commandBus.ExecuteAsync(command, token).ConfigureAwait(false);
                return Unit.Value;
            }, token);
        }

        public async Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken token = default)
        {
            return await ExecuteWithLoggingAsync(command, () => _commandBus.ExecuteAsync(command, token), token);
        }
    }
}