using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunnelWatch.Domain.Gateways;
using TunnelWatch.Domain.Signals;

namespace TunnelWatch.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Mapiranje zadnjih vrijednosti signala na tablicu "latest_signal_values" u bazi podataka, ključ (uređaj, naziv signala).
    /// </summary>
    internal sealed class LatestSignalValueConfiguration : IEntityTypeConfiguration<LatestSignalValue>
    {
        public void Configure(EntityTypeBuilder<LatestSignalValue> builder)
        {
            builder.ToTable("latest_signal_values");
            builder.HasKey(l => new { l.GatewayId, l.SignalName });
            builder.Property(l => l.GatewayId).HasColumnName("gateway_id").HasMaxLength(100);
            builder.Property(l => l.SignalName).HasColumnName("signal_name").HasMaxLength(200);
            builder.Property(l => l.Value).HasColumnName("value");
            builder.Property(l => l.Unit).HasColumnName("unit").HasMaxLength(50);
            builder.Property(l => l.Timestamp).HasColumnName("timestamp");

            builder.HasOne<Gateway>()
                .WithMany()
                .HasForeignKey(l => l.GatewayId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
