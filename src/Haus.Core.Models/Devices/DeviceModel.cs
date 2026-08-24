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
    ushort? NetworkAddress = null,
    LightingModel? Lighting = null
) : IdentityModel
{
    public MetadataModel[] Metadata { get; init; } = Metadata ?? [];

    // MudBlazor's MudDropZone keys rendered items by GetHashCode(); the default
    // record equality includes Metadata (an array, compared by reference), so a
    // refetched instance of the same device got a new hash every time and broke
    // Blazor's diffing during drag-and-drop. Identity equality keeps it stable.
    public virtual bool Equals(DeviceModel? other) => other is not null && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
