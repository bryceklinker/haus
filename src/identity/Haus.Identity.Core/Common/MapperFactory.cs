using AutoMapper;
using Haus.Identity.Core.Accounts;

namespace Haus.Identity.Core.Common
{
    public static class MapperFactory
    {
        public static IMapper CreateMapper()
        {
            var mapperConfig = new MapperConfiguration(c => c.AddProfile<AccountsMapperProfile>());
            return new Mapper(mapperConfig);
        }
    }
}