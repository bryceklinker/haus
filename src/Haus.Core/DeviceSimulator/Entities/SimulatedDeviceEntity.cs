using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;
using Haus.Core.Common.Entities;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.DeviceSimulator;
using Haus.Core.Models.Lighting;

namespace Haus.Core.DeviceSimulator.Entities
{
    public record SimulatedDeviceEntity(string Id = null, DeviceType DeviceType = DeviceType.Unknown, ImmutableArray<Metadata> Metadata = default, LightingModel Lighting = null)
    {
        private static readonly ImmutableArray<Metadata> StandardMetadata = new[]
        {
            new Metadata("simulated", "true")
        }.ToImmutableArray();

        public string Id { get; } = string.IsNullOrEmpty(Id) ? $"{Guid.NewGuid()}" : Id;
        public ImmutableArray<Metadata> Metadata { get; } = Metadata.IsDefaultOrEmpty ? ImmutableArray<Metadata>.Empty : Metadata;
        public LightingModel Lighting { get; } = DeviceType == DeviceType.Light ? Lighting : null;

        public static SimulatedDeviceEntity Create(SimulatedDeviceModel model)
        {
            var metadata = model.Metadata
                .Select(Common.Entities.Metadata.FromModel)
                .Concat(StandardMetadata)
                .ToImmutableArray();
            return new SimulatedDeviceEntity(model.Id, model.DeviceType, metadata);
        }

        public SimulatedDeviceModel ToModel()
        {
            var metadataModels = Metadata.Select(m => new MetadataModel(m.Key, m.Value)).ToArray();
            return new SimulatedDeviceModel(Id, DeviceType, metadataModels, Lighting);
        }

        public DeviceDiscoveredEvent ToDeviceDiscoveredModel()
        {
            var metadata = Metadata.Select(m => m.ToModel()).ToArray();
            return new DeviceDiscoveredEvent(Id, DeviceType, metadata);
        }

        public SimulatedDeviceEntity ChangeLighting(LightingModel model)
        {
            return new(Id, DeviceType, Metadata, model);
        }
    }
}