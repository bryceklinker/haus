using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Events;
using Haus.Core.Devices.Queries;
using Haus.Core.Models.Zigbee.Events;
using Haus.Cqrs;
using Haus.Cqrs.Commands;

namespace Haus.Core.Devices.Commands;

public record PublishZigbeeKnownDeviceSeedCommand : ICommand;

internal class PublishZigbeeKnownDeviceSeedCommandHandler(IHausBus hausBus)
    : ICommandHandler<PublishZigbeeKnownDeviceSeedCommand>
{
    public async Task Handle(PublishZigbeeKnownDeviceSeedCommand request, CancellationToken cancellationToken)
    {
        var seedEntries = await hausBus
            .ExecuteQueryAsync(new GetZigbeeKnownDeviceSeedQuery(), cancellationToken)
            .ConfigureAwait(false);

        var seed = new ZigbeeKnownDeviceSeedEvent(seedEntries.Items);
        await hausBus
            .PublishAsync(new ZigbeeKnownDeviceSeedRequestedEvent(seed), cancellationToken)
            .ConfigureAwait(false);
    }
}
