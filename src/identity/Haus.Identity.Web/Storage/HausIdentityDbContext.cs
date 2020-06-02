using Haus.Identity.Web.Account.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Web.Storage
{
    public class HausIdentityDbContext : IdentityDbContext<HausUser, HausRole, string>
    {
        public HausIdentityDbContext(DbContextOptions<HausIdentityDbContext> options) 
            : base(options)
        {
            
        }
    }
}