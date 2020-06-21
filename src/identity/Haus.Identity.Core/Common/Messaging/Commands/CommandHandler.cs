using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Haus.Identity.Core.Common.Messaging.Commands
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
        public async Task<Unit> Handle(TCommand request, CancellationToken cancellationToken = default)
        {
            await InnerHandle(request, cancellationToken);
            return Unit.Value;
        }

        protected abstract Task InnerHandle(TCommand request, CancellationToken cancellationToken = default);
    }
}