using AutoMapper;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;

namespace Haus.Core.Devices
{
    public class DevicesMappingProfile : Profile
    {
        public DevicesMappingProfile()
        {
            CreateMap<DeviceEntity, DeviceModel>()
                .ForMember(model => model.RoomId, member => member.MapFrom(d => d.Room == null ? default(long?) : d.Room.Id));
            CreateMap<DeviceMetadataEntity, MetadataModel>();
        }
    }
}