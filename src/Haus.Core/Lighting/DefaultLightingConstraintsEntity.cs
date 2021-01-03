using Haus.Core.Common.Entities;
using Haus.Core.Models.Lighting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Lighting
{
    public class DefaultLightingConstraintsEntity : IEntity
    {
        public long Id { get; set; }
        public LightingConstraintsEntity Constraints { get; set; } = new();

        public void UpdateFromModel(LightingConstraintsModel model)
        {
            Constraints.UpdateFromModel(model);
        }
    }
    
    public class DefaultLightingConstraintsEntityConfiguration : IEntityTypeConfiguration<DefaultLightingConstraintsEntity>
    {
        public void Configure(EntityTypeBuilder<DefaultLightingConstraintsEntity> builder)
        {
            builder.HasKey(b => b.Id);
            builder.OwnsOne(b => b.Constraints, LightingConstraintsEntity.Configure);
        }
    }
}