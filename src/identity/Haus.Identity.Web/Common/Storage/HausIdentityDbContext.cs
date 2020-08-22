using Haus.Identity.Web.Users.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Web.Common.Storage
{
    public class HausIdentityDbContext : IdentityDbContext<HausUser, HausRole, string>
    {
        public HausIdentityDbContext(DbContextOptions<HausIdentityDbContext> options)
            : base(options)
        {
            
        }
    }
}