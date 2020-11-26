using AutoMapper;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;

namespace Haus.Core.Devices
{
    public class DevicesMappingProfile : Profile
    {
        public DevicesMappingProfile()
        {
            CreateMap<DeviceEntity, DeviceModel>();
            CreateMap<DeviceMetadataEntity, DeviceMetadataModel>();
        }
    }
}