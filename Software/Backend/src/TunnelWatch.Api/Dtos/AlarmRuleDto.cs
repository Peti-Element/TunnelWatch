using TunnelWatch.Domain.Alarms;

namespace TunnelWatch.Api.Dtos
{
    /// <summary>
    /// Pravilo alarma u odgovorima /api/rules.
    /// </summary>
    public sealed record AlarmRuleDto(
        int Id,
        string GatewayId,
        string SignalName,
        RuleOperator Operator,
        double Threshold,
        int DurationSeconds,
        double Hysteresis,
        AlarmSeverity Severity,
        int RepeatSuppressionSeconds,
        bool Enabled);
}
