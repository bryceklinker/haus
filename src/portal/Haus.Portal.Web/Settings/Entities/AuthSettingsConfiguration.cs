using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Portal.Web.Settings.Entities
{
    public class AuthSettingsConfiguration : IEntityTypeConfiguration<AuthSettings>
    {
        public void Configure(EntityTypeBuilder<AuthSettings> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ClientId);
            builder.Property(c => c.ClientSecret);
        }
    }
}