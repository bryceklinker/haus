using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Haus.Core.DeviceSimulator.Entities;

namespace Haus.Core.DeviceSimulator.State
{
    public interface IDeviceSimulatorState
    {
        ImmutableArray<SimulatedDeviceEntity> Devices { get; }
        DeviceSimulatorState AddSimulatedDevice(SimulatedDeviceEntity entity);
    }

    public record DeviceSimulatorState : IDeviceSimulatorState
    {
        public static readonly DeviceSimulatorState Initial = new();

        public ImmutableArray<SimulatedDeviceEntity> Devices { get; }

        private DeviceSimulatorState(IEnumerable<SimulatedDeviceEntity> devices = null)
        {
            Devices = (devices ?? Enumerable.Empty<SimulatedDeviceEntity>()).ToImmutableArray();
        }
        
        public DeviceSimulatorState AddSimulatedDevice(SimulatedDeviceEntity entity)
        {
            return new(Devices.Add(entity));
        }
    }
}