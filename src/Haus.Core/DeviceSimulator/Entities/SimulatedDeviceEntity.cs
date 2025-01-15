using System;
using System.Collections.Immutable;
using System.Linq;
using Haus.Core.Common.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.DeviceSimulator;
using Haus.Core.Models.Lighting;

namespace Haus.Core.DeviceSimulator.Entities;

public record SimulatedDeviceEntity(
    string? Id = null,
    DeviceType DeviceType = DeviceType.Unknown,
    bool IsOccupied = false,
    Metadata[]? Metadata = null,
    LightingModel? Lighting = null)
{
    private static readonly ImmutableArray<Metadata> StandardMetadata = [
        ..new[]
        {
            new Metadata("simulated", "true")
        }
    ];

    public string Id { get; } = string.IsNullOrEmpty(Id) ? $"{Guid.NewGuid()}" : Id;

    public Metadata[] Metadata { get; } = Metadata ?? [];

    public LightingModel? Lighting { get; private init; } = DeviceType == DeviceType.Light ? Lighting : null;

    public static SimulatedDeviceEntity Create(SimulatedDeviceModel model)
    {
        var metadata = model.Metadata
            .Select(Common.Entities.Metadata.FromModel)
            .Concat(StandardMetadata)
            .ToArray();
        return new SimulatedDeviceEntity(model.Id, model.DeviceType, Metadata: metadata);
    }

    public SimulatedDeviceModel ToModel()
    {
        var metadataModels = Metadata.Select(m => new MetadataModel(m.Key, m.Value)).ToArray();
        return new SimulatedDeviceModel(Id, DeviceType, IsOccupied, metadataModels, Lighting);
    }

    public DeviceDiscoveredEvent ToDeviceDiscoveredModel()
    {
        var metadata = Metadata.Select(m => m.ToModel()).ToArray();
        return new DeviceDiscoveredEvent(Id, DeviceType, metadata);
    }

    public SimulatedDeviceEntity ChangeLighting(LightingModel model)
    {
        return this with { Lighting = model };
    }

    public SimulatedDeviceEntity ChangeOccupancy()
    {
        return this with { IsOccupied = !IsOccupied };
    }
}