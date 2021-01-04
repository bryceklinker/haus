using System;

namespace Haus.Core.Models.DeviceSimulator
{
    public record DeviceSimulatorStateModel(SimulatedDeviceModel[] Devices)
    {
        public SimulatedDeviceModel[] Devices { get; } = Devices ?? Array.Empty<SimulatedDeviceModel>();
    }
}