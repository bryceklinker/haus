using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Haus.Cqrs.Queries;

public interface IQueryBus
{
    Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken token = default);
}

internal class QueryBus(IMediator mediator) : IQueryBus
{
    public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken token = default)
    {
        return await mediator.Send(query, token).ConfigureAwait(false);
    }
}
