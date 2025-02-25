using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Cqrs.Queries;

namespace Haus.Core.Devices.Queries;

public record GetLightTypesQuery : IQuery<ListResult<LightType>>;

internal class GetLightTypesQueryHandler : IQueryHandler<GetLightTypesQuery, ListResult<LightType>>
{
    public Task<ListResult<LightType>> Handle(GetLightTypesQuery request, CancellationToken cancellationToken)
    {
        var values = Enum.GetValues<LightType>().Where(type => type != LightType.None).ToListResult();

        return Task.FromResult(values);
    }
}
