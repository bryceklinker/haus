using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Lighting;

namespace Haus.Core.Models.DeviceSimulator;

public record SimulatedDeviceModel(
    string? Id = null,
    DeviceType DeviceType = DeviceType.Unknown,
    bool IsOccupied = false,
    MetadataModel[]? Metadata = null,
    LightingModel? Lighting = null)
{
    public MetadataModel[] Metadata { get; init; } = Metadata ?? [];
    public LightingModel? Lighting { get; init; } = DeviceType == DeviceType.Light ? Lighting : null;
}