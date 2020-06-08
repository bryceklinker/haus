using Haus.Identity.Core.Accounts.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Core.Storage
{
    public class HausIdentityDbContext : IdentityDbContext<HausUser, HausRole, string>
    {
        public HausIdentityDbContext(DbContextOptions<HausIdentityDbContext> options) 
            : base(options)
        {
            
        }
    }
}