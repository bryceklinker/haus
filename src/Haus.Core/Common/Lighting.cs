using Haus.Core.Models.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Common
{
    public class Lighting
    {
        public LightingState? State { get; set; }
        public double? Brightness { get; set; }
        public double? Temperature { get; set; }
        public LightingColor Color { get; set; }

        public static Lighting FromModel(LightingModel model)
        {
            return new Lighting
            {
                State = model.State,
                Brightness = model.Brightness,
                Color = LightingColor.FromModel(model.Color),
                Temperature = model.Temperature
            };
        }
        
        public static void Configure<TEntity>(OwnedNavigationBuilder<TEntity, Lighting> builder) 
            where TEntity : class
        {
            builder.Property(l => l.State).HasConversion<string>();
            builder.OwnsOne(l => l.Color);
        }
    }
}