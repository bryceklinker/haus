using Haus.Core.Models.Common;
using Haus.Core.Models.Lighting;
using Humanizer;

namespace Haus.Core.Models.Devices;

public record DeviceModel(
    long Id = -1,
    long? RoomId = null,
    string ExternalId = "",
    string? Name = null,
    DeviceType DeviceType = DeviceType.Unknown,
    LightType LightType = LightType.None,
    MetadataModel[]? Metadata = null,
    LightingModel? Lighting = null
) : IdentityModel
{
    public MetadataModel[] Metadata { get; init; } = Metadata ?? [];
}
