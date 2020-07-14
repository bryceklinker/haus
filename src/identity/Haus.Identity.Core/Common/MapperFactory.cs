using AutoMapper;
using Haus.Identity.Core.Users;

namespace Haus.Identity.Core.Common
{
    public static class MapperFactory
    {
        public static IMapper CreateMapper()
        {
            var mapperConfig = new MapperConfiguration(c => c.AddProfile<UsersMapperProfile>());
            return new Mapper(mapperConfig);
        }
    }
}