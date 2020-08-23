using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Haus.Cqrs.Commands
{
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
        where TCommand : ICommand
    {
        
    }

    public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        
    }

    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Unit> Handle(TCommand request, CancellationToken token = default)
        {
            await InnerHandle(request, token).ConfigureAwait(false);
            return Unit.Value;
        }

        protected abstract Task InnerHandle(TCommand command, CancellationToken token = default);
    }
}