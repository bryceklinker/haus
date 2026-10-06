using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Resolvers;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Haus.Core.Devices.Commands;

public record ReclassifyUnknownDevicesCommand : ICommand;

// Mirrors BackfillDeviceNetworkAddressesCommand: a device whose Vendor/Model was stored at
// original pairing can still be reachable here even when the Zigbee coordinator's live
// KnownDeviceTable no longer carries it (it only repopulates on a fresh ZDP rediscovery/announce
// since process start) -- so this re-resolves straight from already-persisted metadata instead.
internal class ReclassifyUnknownDevicesCommandHandler(
    HausDbContext context,
    IDeviceTypeResolver deviceTypeResolver,
    IDomainEventBus domainEventBus,
    ILogger<ReclassifyUnknownDevicesCommandHandler> logger
) : ICommandHandler<ReclassifyUnknownDevicesCommand>
{
    private const string VendorMetadataKey = "vendor";
    private const string ModelMetadataKey = "model";

    public async Task Handle(ReclassifyUnknownDevicesCommand request, CancellationToken cancellationToken)
    {
        var unknownDevices = await context
            .Set<DeviceEntity>()
            .Include(d => d.Metadata)
            .Where(d => d.DeviceType == DeviceType.Unknown)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var device in unknownDevices)
            Reclassify(device);

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    private void Reclassify(DeviceEntity device)
    {
        var vendor = GetMetadataValue(device, VendorMetadataKey);
        var model = GetMetadataValue(device, ModelMetadataKey);
        var resolvedType = deviceTypeResolver.Resolve(vendor, model);
        if (resolvedType == DeviceType.Unknown)
            return;

        device.ReclassifyIfResolvable(resolvedType, domainEventBus);
        logger.LogInformation(
            "Reclassified device {@Id} from Unknown to {@DeviceType} using stored vendor/model metadata",
            device.Id,
            resolvedType
        );
    }

    private static string? GetMetadataValue(DeviceEntity device, string key)
    {
        return device.Metadata.FirstOrDefault(m => m.Key == key)?.Value;
    }
}
