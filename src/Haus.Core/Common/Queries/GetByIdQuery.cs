using Haus.Cqrs.Queries;

namespace Haus.Core.Common.Queries;

public record GetByIdQuery<T>(long Id) : IQuery<T>;
