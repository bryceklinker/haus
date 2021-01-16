using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Discovery.Entities
{
    public class DiscoveryEntityConfiguration : IEntityTypeConfiguration<DiscoveryEntity>
    {
        public void Configure(EntityTypeBuilder<DiscoveryEntity> builder)
        {
            builder.ToTable("Discovery");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.State).IsRequired().HasConversion<string>();
        }
    }
}