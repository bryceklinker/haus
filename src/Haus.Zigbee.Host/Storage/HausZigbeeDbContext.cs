using Haus.Zigbee.Host.Storage.Entities;
using Microsoft.EntityFrameworkCore;

namespace Haus.Zigbee.Host.Storage;

public class HausZigbeeDbContext(DbContextOptions<HausZigbeeDbContext> options) : DbContext(options)
{
    public DbSet<KnownZigbeeDeviceEntity> KnownDevices => Set<KnownZigbeeDeviceEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HausZigbeeDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
