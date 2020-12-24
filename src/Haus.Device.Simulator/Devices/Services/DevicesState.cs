using System.Collections.Immutable;
using Haus.Core.Models.Devices;

namespace Haus.Device.Simulator.Devices.Services
{
    public interface IDevicesState
    {
        ImmutableDictionary<string, DeviceModel> DevicesById { get; }
        ImmutableArray<DeviceModel> Devices { get; }
        IDevicesState AddDevice(DeviceModel model);
        IDevicesState UpdateDevice(DeviceModel model);
        DeviceModel GetDeviceById(string id);
        IDevicesState Clear();
    }
    
    public class DevicesState : IDevicesState
    {
        public ImmutableDictionary<string, DeviceModel> DevicesById { get; set; } = ImmutableDictionary<string, DeviceModel>.Empty;
        public ImmutableArray<DeviceModel> Devices => DevicesById.Values.ToImmutableArray();

        public DevicesState()
        {
        }

        private DevicesState(ImmutableDictionary<string, DeviceModel> devicesById)
        {
            DevicesById = devicesById;
        }
        
        public IDevicesState AddDevice(DeviceModel model)
        {
            return new DevicesState(DevicesById.Add(model.ExternalId, model));
        }

        public IDevicesState UpdateDevice(DeviceModel model)
        {
            return new DevicesState(DevicesById.SetItem(model.ExternalId, model));
        }

        public IDevicesState Clear()
        {
            return new DevicesState();
        }

        public DeviceModel GetDeviceById(string id)
        {
            return DevicesById.ContainsKey(id)
                ? DevicesById[id]
                : null;
        }
    }
}