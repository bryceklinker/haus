using Haus.Cqrs.Queries;

namespace Haus.Core.Common.Queries
{
    public class GetByIdQuery<T> : IQuery<T>
    {
        public long Id { get; }

        public GetByIdQuery(long id)
        {
            Id = id;
        }
    }
}