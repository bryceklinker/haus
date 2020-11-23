using MediatR;

namespace Haus.Core.Common.Queries
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
        
    }
}