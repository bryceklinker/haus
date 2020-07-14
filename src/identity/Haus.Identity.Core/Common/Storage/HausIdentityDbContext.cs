using Haus.Identity.Core.Users.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Core.Common.Storage
{
    public class HausIdentityDbContext : IdentityDbContext<HausUser, HausRole, string>
    {
        public HausIdentityDbContext(DbContextOptions<HausIdentityDbContext> options) 
            : base(options)
        {
            
        }
    }
}