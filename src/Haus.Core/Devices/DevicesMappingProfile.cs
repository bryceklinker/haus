using System.Linq;
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
                .ConstructUsing(e =>
                    new DeviceModel(e.Id, e.Room == null ? default(long?) : e.Room.Id, e.ExternalId, e.Name, e.DeviceType, e.Metadata.Select(m => new MetadataModel(m.Key, m.Value)).ToArray()));
            CreateMap<DeviceMetadataEntity, MetadataModel>();
        }
    }
}