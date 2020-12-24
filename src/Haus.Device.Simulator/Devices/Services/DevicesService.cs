using System;
using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Mqtt.Client;

namespace Haus.Device.Simulator.Devices.Services
{
    public interface IDevicesService
    {
        Task AddDevice(DeviceModel model);
        void ClearState();
        ListResult<DeviceModel> GetAllDevices();
    }
    
    public class DevicesService : IDevicesService
    {
        private readonly IDevicesStore _store;
        private readonly IHausMqttClientFactory _mqttClientFactory;

        public DevicesService(IDevicesStore store, IHausMqttClientFactory mqttClientFactory)
        {
            _store = store;
            _mqttClientFactory = mqttClientFactory;
        }

        public async Task AddDevice(DeviceModel model)
        {
            var device = CreateDeviceFromModel(model);
            var state = _store.Current.AddDevice(device);
            var client = await _mqttClientFactory.CreateClient();
            await client.PublishHausEventAsync(new DeviceDiscoveredModel
            {
                Id = device.ExternalId,
                Metadata = device.Metadata,
                DeviceType = device.DeviceType
            });
            _store.Next(state);
        }

        public void ClearState()
        {
            var state = _store.Current.Clear();
            _store.Next(state);
        }

        public ListResult<DeviceModel> GetAllDevices()
        {
            var state = _store.Current.Devices;
            return state.ToListResult();
        }

        private static DeviceModel CreateDeviceFromModel(DeviceModel model)
        {
            return new()
            {
                DeviceType = model.DeviceType,
                ExternalId = $"{Guid.NewGuid()}",
                Metadata = model.Metadata
                    .Append(new DeviceMetadataModel("SIMULATED", true.ToString()))
                    .ToArray()
            };
        }
    }
}