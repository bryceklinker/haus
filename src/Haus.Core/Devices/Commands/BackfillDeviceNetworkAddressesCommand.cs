using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Cqrs.Commands;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Devices.Commands;

public record BackfillDeviceNetworkAddressesCommand : ICommand;

internal class BackfillDeviceNetworkAddressesCommandHandler(HausDbContext context)
    : ICommandHandler<BackfillDeviceNetworkAddressesCommand>
{
    private const string LegacyNetworkAddressMetadataKey = "network_address";

    public async Task Handle(BackfillDeviceNetworkAddressesCommand request, CancellationToken cancellationToken)
    {
        var devicesMissingNetworkAddress = await context
            .Set<DeviceEntity>()
            .Include(d => d.Metadata)
            .Where(d => d.NetworkAddress == null)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var device in devicesMissingNetworkAddress)
        {
            var legacyValue = device.Metadata.FirstOrDefault(m => m.Key == LegacyNetworkAddressMetadataKey)?.Value;
            if (legacyValue == null || !ushort.TryParse(legacyValue, out var networkAddress))
                continue;

            device.NetworkAddress = networkAddress;
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
