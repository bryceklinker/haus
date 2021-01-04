using System.Collections.Immutable;
using System.Linq;
using Haus.Core.DeviceSimulator.Entities;
using Haus.Core.Models.DeviceSimulator;

namespace Haus.Core.DeviceSimulator.State
{
    public interface IDeviceSimulatorState
    {
        ImmutableArray<SimulatedDeviceEntity> Devices { get; }
        IDeviceSimulatorState AddSimulatedDevice(SimulatedDeviceEntity entity);
        IDeviceSimulatorState Reset();
        DeviceSimulatorStateModel ToModel();
    }

    public record DeviceSimulatorState : IDeviceSimulatorState
    {
        public static readonly DeviceSimulatorState Initial = new(ImmutableArray<SimulatedDeviceEntity>.Empty);

        public ImmutableArray<SimulatedDeviceEntity> Devices { get; }

        public DeviceSimulatorState(ImmutableArray<SimulatedDeviceEntity> devices)
        {
            Devices = devices;
        }

        public IDeviceSimulatorState Reset()
        {
            return Initial;
        }

        public IDeviceSimulatorState AddSimulatedDevice(SimulatedDeviceEntity entity)
        {
            return new DeviceSimulatorState(Devices.Add(entity));
        }

        public DeviceSimulatorStateModel ToModel()
        {
            var devices = Devices.Select(d => d.ToModel()).ToArray();
            return new DeviceSimulatorStateModel(devices);
        }
    }
}