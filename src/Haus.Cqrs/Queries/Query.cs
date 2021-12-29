using MediatR;

namespace Haus.Cqrs.Queries;

public interface IQuery<out TResult> : IRequest<TResult>
{
}