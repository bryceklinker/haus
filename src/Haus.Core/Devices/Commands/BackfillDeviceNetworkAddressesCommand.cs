using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Cqrs.Commands;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Devices.Commands;

public record BackfillDeviceNetworkAddressesCommand : ICommand;

// PR #62 promoted NetworkAddress to a typed column but never backfilled it, so every device that
// was known before that migration has NetworkAddress == null even though its network address was
// already captured in the legacy Metadata["network_address"] entry -- this reconciles the two on
// every startup, which is what makes it safe to run repeatedly.
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

        var backfilled = false;
        foreach (var device in devicesMissingNetworkAddress)
        {
            var legacyValue = device.Metadata.FirstOrDefault(m => m.Key == LegacyNetworkAddressMetadataKey)?.Value;
            if (legacyValue == null || !ushort.TryParse(legacyValue, out var networkAddress))
                continue;

            device.NetworkAddress = networkAddress;
            backfilled = true;
        }

        if (backfilled)
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
