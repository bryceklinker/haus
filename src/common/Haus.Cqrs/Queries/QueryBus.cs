using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Haus.Cqrs.Queries
{
    public interface IQueryBus
    {
        Task<TResult> Execute<TResult>(IQuery<TResult> query, CancellationToken token = default);
    }
    public class QueryBus : IQueryBus
    {
        private readonly IMediator _mediator;

        public QueryBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TResult> Execute<TResult>(IQuery<TResult> query, CancellationToken token = default)
        {
            return await _mediator.Send(query, token).ConfigureAwait(false);
        }
    }
}