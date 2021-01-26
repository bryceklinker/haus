using System.Collections.Immutable;
using System.Linq;
using Haus.Core.DeviceSimulator.Entities;
using Haus.Core.Models.DeviceSimulator;
using Haus.Core.Models.Lighting;

namespace Haus.Core.DeviceSimulator.State
{
    public interface IDeviceSimulatorState
    {
        DeviceSimulatorStateModel ToModel();
        ImmutableArray<SimulatedDeviceEntity> Devices { get; }
        SimulatedDeviceEntity GetDeviceById(string deviceId);
        IDeviceSimulatorState AddSimulatedDevice(SimulatedDeviceEntity entity);
        IDeviceSimulatorState Reset();
        IDeviceSimulatorState ChangeDeviceLighting(string deviceId, LightingModel lighting);
        IDeviceSimulatorState ChangeOccupancy(string deviceId);
    }

    public record DeviceSimulatorState(ImmutableArray<SimulatedDeviceEntity> Devices) : IDeviceSimulatorState
    {
        public static readonly DeviceSimulatorState Initial = new(ImmutableArray<SimulatedDeviceEntity>.Empty);

        public ImmutableArray<SimulatedDeviceEntity> Devices { get; private init; } = Devices.IsDefaultOrEmpty ? ImmutableArray<SimulatedDeviceEntity>.Empty :  Devices;

        public IDeviceSimulatorState Reset()
        {
            return Initial;
        }

        public IDeviceSimulatorState AddSimulatedDevice(SimulatedDeviceEntity entity)
        {
            return this with {Devices = Devices.Add(entity)};
        }

        public IDeviceSimulatorState ChangeDeviceLighting(string deviceId, LightingModel lighting)
        {
            var device = GetDeviceById(deviceId);
            if (device == null)
                return this;
            
            var updatedDevice = device.ChangeLighting(lighting);
            return CreateStateReplacingDevice(device, updatedDevice);
        }

        public IDeviceSimulatorState ChangeOccupancy(string deviceId)
        {
            var device = GetDeviceById(deviceId);
            if (device == null)
                return this;

            var updatedDevice = device.ChangeOccupancy();
            return CreateStateReplacingDevice(device, updatedDevice);
        }

        public DeviceSimulatorStateModel ToModel()
        {
            var devices = Devices.Select(d => d.ToModel()).ToArray();
            return new DeviceSimulatorStateModel(devices);
        }

        public SimulatedDeviceEntity GetDeviceById(string deviceId)
        {
            return Devices.FirstOrDefault(d => d.Id == deviceId);
        }
        
        private IDeviceSimulatorState CreateStateReplacingDevice(SimulatedDeviceEntity oldDevice, SimulatedDeviceEntity newDevice)
        {
            return this with {Devices = Devices.Remove(oldDevice).Add(newDevice)};
        }
    }
}