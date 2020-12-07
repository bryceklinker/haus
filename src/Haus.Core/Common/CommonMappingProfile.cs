using AutoMapper;
using Haus.Core.Common.Entities;
using Haus.Core.Models.Common;

namespace Haus.Core.Common
{
    public class CommonMappingProfile : Profile
    {
        public CommonMappingProfile()
        {
            CreateMap<Lighting, LightingModel>();
            CreateMap<LightingColor, LightingColorModel>();
        }
    }
}