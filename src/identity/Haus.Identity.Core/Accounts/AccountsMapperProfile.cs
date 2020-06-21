using AutoMapper;
using Haus.Identity.Core.Accounts.Entities;
using Haus.Identity.Core.Accounts.Models;

namespace Haus.Identity.Core.Accounts
{
    public class AccountsMapperProfile : Profile
    {
        public AccountsMapperProfile()
        {
            CreateMap<HausUser, HausUserModel>();
        }
    }
}