using AutoMapper;
using Haus.Identity.Core.Users.Entities;
using Haus.Identity.Core.Users.Models;

namespace Haus.Identity.Core.Users
{
    public class UsersMapperProfile : Profile
    {
        public UsersMapperProfile()
        {
            CreateMap<HausUser, HausUserModel>();
        }
    }
}