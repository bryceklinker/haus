using Microsoft.EntityFrameworkCore;

namespace Haus.Portal.Web.Common.Storage
{
    public class HausPortalDbContext : DbContext
    {
        public HausPortalDbContext(DbContextOptions<HausPortalDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}