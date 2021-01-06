using System;
using System.Linq.Expressions;
using Haus.Core.Common.Entities;
using Haus.Core.Models.Discovery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Discovery.Entities
{
    public class DiscoveryEntity : Entity
    {
        public static readonly Expression<Func<DiscoveryEntity, DiscoveryModel>> ToModelExpression = d => new DiscoveryModel(d.State);
        public static readonly Lazy<Func<DiscoveryEntity, DiscoveryModel>> ToModelFunc = new(ToModelExpression.Compile);
        public DiscoveryState State { get; set; }
        
        public DiscoveryEntity()
            : this(0)
        {
            
        }

        public DiscoveryEntity(long id = 0, DiscoveryState state = DiscoveryState.Disabled)
        {
            Id = id;
            State = state;
        }

        public DiscoveryModel ToModel() => ToModelFunc.Value.Invoke(this);
        
        public void Start()
        {
            State = DiscoveryState.Enabled;
        }

        public void Stop()
        {
            State = DiscoveryState.Disabled;
        }
    }

    public class DiscoveryEntityConfiguration : IEntityTypeConfiguration<DiscoveryEntity>
    {
        public void Configure(EntityTypeBuilder<DiscoveryEntity> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.State).IsRequired().HasConversion<string>();
        }
    }
}