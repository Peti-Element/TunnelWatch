using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunnelWatch.Domain.Alarms;

namespace TunnelWatch.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Mapiranje pravila alarma na tablicu "alarm_rules". Operator i ozbiljnost spremaju se kao tekst (npr. "GreaterThan", "Critical").
    /// </summary>
    internal sealed class AlarmRuleConfiguration : IEntityTypeConfiguration<AlarmRule>
    {
        public void Configure(EntityTypeBuilder<AlarmRule> builder)
        {
            builder.ToTable("alarm_rules");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            builder.Property(r => r.GatewayId).HasColumnName("gateway_id").HasMaxLength(100);
            builder.Property(r => r.SignalName).HasColumnName("signal_name").HasMaxLength(200);
            builder.Property(r => r.Operator).HasColumnName("operator").HasConversion<string>().HasMaxLength(30);
            builder.Property(r => r.Threshold).HasColumnName("threshold");
            builder.Property(r => r.DurationSeconds).HasColumnName("duration_seconds");
            builder.Property(r => r.Hysteresis).HasColumnName("hysteresis");
            builder.Property(r => r.Severity).HasColumnName("severity").HasConversion<string>().HasMaxLength(20);
            builder.Property(r => r.RepeatSuppressionSeconds).HasColumnName("repeat_suppression_seconds");
            builder.Property(r => r.Enabled).HasColumnName("enabled");
        }
    }
}
