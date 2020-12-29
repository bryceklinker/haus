using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Haus.Core.Common.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
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

        private SimulatedDeviceEntity(string id, DeviceType deviceType, IEnumerable<Metadata> metadata = null)
        {
            Id = id;
            DeviceType = deviceType;
            Metadata = (metadata ?? Enumerable.Empty<Metadata>()).ToImmutableArray();
        }

        public static SimulatedDeviceEntity Create(CreateSimulatedDeviceModel model)
        {
            var metadata = model.Metadata
                .Select(Common.Entities.Metadata.FromModel)
                .Concat(StandardMetadata)
                .ToImmutableArray();
            return new SimulatedDeviceEntity($"{Guid.NewGuid()}", model.DeviceType, metadata);
        }

        public DeviceDiscoveredModel ToDeviceDiscoveredModel()
        {
            return new()
            {
                Id = Id,
                DeviceType = DeviceType,
                Metadata = Metadata.Select(m => m.ToModel()).ToArray()
            };
        }
    }
}