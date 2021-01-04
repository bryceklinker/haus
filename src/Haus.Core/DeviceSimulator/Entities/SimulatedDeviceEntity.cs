using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;
using Haus.Core.Common.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.DeviceSimulator;

namespace Haus.Core.DeviceSimulator.Entities
{
    public class SimulatedDeviceEntity
    {
        private static readonly ImmutableArray<Metadata> StandardMetadata = new[]
        {
            new Metadata("simulated", "true")
        }.ToImmutableArray();
        
        public string Id { get; }
        public DeviceType DeviceType { get; }
        public ImmutableArray<Metadata> Metadata { get; }

        public SimulatedDeviceEntity(string id, DeviceType deviceType, ImmutableArray<Metadata> metadata)
        {
            Id = id;
            DeviceType = deviceType;
            Metadata = metadata;
        }

        public static SimulatedDeviceEntity Create(CreateSimulatedDeviceModel model)
        {
            var metadata = model.Metadata
                .Select(Common.Entities.Metadata.FromModel)
                .Concat(StandardMetadata)
                .ToImmutableArray();
            return new SimulatedDeviceEntity($"{Guid.NewGuid()}", model.DeviceType, metadata);
        }

        public DeviceDiscoveredEvent ToDeviceDiscoveredModel()
        {
            var metadata = Metadata.Select(m => m.ToModel()).ToArray();
            return new DeviceDiscoveredEvent(Id, DeviceType, metadata);
        }
    }
}