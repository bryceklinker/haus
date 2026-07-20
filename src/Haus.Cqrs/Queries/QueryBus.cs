using System;
using System.Threading;
using System.Threading.Tasks;

namespace Haus.Cqrs.Queries;

public interface IQueryBus
{
    Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken token = default);
}

internal class QueryBus(IServiceProvider services) : IQueryBus
{
    public Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken token = default)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        return HandlerInvoker.InvokeAsync<TResult>(services, handlerType, query, token);
    }
}
