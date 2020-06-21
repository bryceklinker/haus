using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Haus.Identity.Core.Common.Messaging
{
    public interface ICommand : IRequest
    {

    }

    public interface ICommand<out TResult> : IRequest<TResult>
    {

    }
    
    public interface ICommandBus
    {
        Task ExecuteCommand(ICommand command, CancellationToken token = default);

        Task<TResult> ExecuteCommand<TResult>(ICommand<TResult> command, CancellationToken token = default);
    }

    public class CommandBus : ICommandBus
    {
        private readonly IMediator _mediator;

        public CommandBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ExecuteCommand(ICommand command, CancellationToken token = default)
        {
            await _mediator.Send(command, token);
        }

        public async Task<TResult> ExecuteCommand<TResult>(ICommand<TResult> command, CancellationToken token = default)
        {
            return await _mediator.Send<TResult>(command, token);
        }
    }
}