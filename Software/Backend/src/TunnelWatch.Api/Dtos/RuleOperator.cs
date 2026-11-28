using System.Text.Json.Serialization;

namespace TunnelWatch.Api.Dtos
{
    /// <summary>
    /// Operator pravila u JSON-u API-ja, zapisan kao simbol.
    /// </summary>
    public enum RuleOperator
    {
        [JsonStringEnumMemberName(">")]
        GreaterThan,

        [JsonStringEnumMemberName(">=")]
        GreaterThanOrEqual,

        [JsonStringEnumMemberName("<")]
        LessThan,

        [JsonStringEnumMemberName("<=")]
        LessThanOrEqual,
    }
}
