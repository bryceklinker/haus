using MediatR;

namespace Haus.Core.Common.Commands
{
    public interface ICommand : IRequest
    {
        
    }

    public interface ICommand<out TResult> : IRequest<TResult>
    {
        
    }
}