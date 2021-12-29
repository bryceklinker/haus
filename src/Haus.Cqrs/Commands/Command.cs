using MediatR;

namespace Haus.Cqrs.Commands;

public interface ICommand : IRequest
{
}

public interface ICommand<out TResult> : IRequest<TResult>
{
}