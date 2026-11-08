using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunnelWatch.Domain.Gateways;

namespace TunnelWatch.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Mapiranje uređaja na tablicu "gateways" u bazi podataka.
    /// </summary>
    internal sealed class GatewayConfiguration : IEntityTypeConfiguration<Gateway>
    {
        public void Configure(EntityTypeBuilder<Gateway> builder)
        {
            builder.ToTable("gateways");
            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id).HasColumnName("id").HasMaxLength(100);
            builder.Property(g => g.LastMessageAt).HasColumnName("last_message_at");
        }
    }
}
