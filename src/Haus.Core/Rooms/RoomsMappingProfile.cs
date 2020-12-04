using AutoMapper;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Entities;

namespace Haus.Core.Rooms
{
    public class RoomsMappingProfile : Profile
    {
        public RoomsMappingProfile()
        {
            CreateMap<RoomEntity, RoomModel>();
        }
    }
}