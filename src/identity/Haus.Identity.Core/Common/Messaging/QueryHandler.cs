using MediatR;

namespace Haus.Identity.Core.Common.Messaging
{
    public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        
    }
}