using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunnelWatch.Domain.Gateways;
using TunnelWatch.Domain.Signals;

namespace TunnelWatch.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Mapiranje povijesti na tablica "signal_history", ključ (uređaj, naziv signala, minuta).
    /// </summary>
    internal sealed class SignalMinuteHistoryConfiguration : IEntityTypeConfiguration<SignalMinuteHistory>
    {
        public void Configure(EntityTypeBuilder<SignalMinuteHistory> builder)
        {
            builder.ToTable("signal_history");
            builder.HasKey(s => new { s.GatewayId, s.SignalName, s.Minute });
            builder.Property(s => s.GatewayId).HasColumnName("gateway_id").HasMaxLength(100);
            builder.Property(s => s.SignalName).HasColumnName("signal_name").HasMaxLength(200);
            builder.Property(s => s.Minute).HasColumnName("minute");
            builder.Property(s => s.Average).HasColumnName("average");
            builder.Property(s => s.Minimum).HasColumnName("minimum");
            builder.Property(s => s.Maximum).HasColumnName("maximum");
            builder.Property(s => s.SampleCount).HasColumnName("sample_count");
            builder.Property(s => s.Unit).HasColumnName("unit").HasMaxLength(50);
            builder.HasOne<Gateway>()
                .WithMany()
                .HasForeignKey(s => s.GatewayId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
