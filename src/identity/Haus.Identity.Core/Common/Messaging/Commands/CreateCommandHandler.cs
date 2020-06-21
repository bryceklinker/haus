using System.Threading;
using System.Threading.Tasks;

namespace Haus.Identity.Core.Common.Messaging.Commands
{
    public abstract class CreateCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult> 
        where TCommand : ICommand<TResult>
        where TResult : CommandResult
    {
        public async Task<TResult> Handle(TCommand request, CancellationToken cancellationToken)
        {
            if (await DoesExist(request))
                return CreateDuplicateFailureResult(request);

            return await Create(request, cancellationToken);
        }

        protected abstract Task<TResult> Create(TCommand command, CancellationToken cancellationToken);

        protected abstract Task<bool> DoesExist(TCommand command);

        protected abstract TResult CreateDuplicateFailureResult(TCommand command);
    }
}