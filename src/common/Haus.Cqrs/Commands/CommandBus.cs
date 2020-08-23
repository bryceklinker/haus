using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Haus.Cqrs.Commands
{
    public interface ICommandBus
    {
        Task Execute(ICommand command, CancellationToken token = default);
        Task<TResult> Execute<TResult>(ICommand<TResult> command, CancellationToken token = default);
    }
    public class CommandBus : ICommandBus
    {
        private readonly IMediator _mediator;

        public CommandBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Execute(ICommand command, CancellationToken token = default)
        {
            await _mediator.Send(command, token).ConfigureAwait(false);
        }

        public async Task<TResult> Execute<TResult>(ICommand<TResult> command, CancellationToken token = default)
        {
            return await _mediator.Send(command, token).ConfigureAwait(false);
        }
    }
}