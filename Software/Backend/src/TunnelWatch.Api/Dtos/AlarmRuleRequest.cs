using System.ComponentModel.DataAnnotations;
using TunnelWatch.Domain.Alarms;

namespace TunnelWatch.Api.Dtos
{
    /// <summary>
    /// Tijelo zahtjeva za POST i PUT /api/rules. Atributi provjeravaju podatke prije kontrolera:
    /// ako nešto ne valja, API vraća 400 s popisom grešaka po poljima.
    /// </summary>
    public sealed class AlarmRuleRequest
    {
        /// <summary>Uređaj, npr. "tdce-01".</summary>
        [Required]
        [StringLength(100)]
        public string GatewayId { get; init; } = string.Empty;

        /// <summary>Signal, npr. "engine_coolant_temperature".</summary>
        [Required]
        [StringLength(200)]
        public string SignalName { get; init; } = string.Empty;

        [Required]
        public RuleOperator? Operator { get; init; }

        /// <summary>Prag u jedinici signala.</summary>
        [Required]
        public double? Threshold { get; init; }

        /// <summary>Koliko dugo uvjet mora vrijediti, u sekundama (0 ili više).</summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Trajanje ne smije biti negativno.")]
        public int? DurationSeconds { get; init; }

        /// <summary>Histereza u jedinici signala (0 ili više).</summary>
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Histereza ne smije biti negativna.")]
        public double? Hysteresis { get; init; }

        /// <summary>"Info", "Warning" ili "Critical".</summary>
        [Required]
        public AlarmSeverity? Severity { get; init; }

        /// <summary>Najkraći razmak između dva alarma istog pravila, u sekundama (0 ili više).</summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Vrijeme sprečavanja ponavljanja ne smije biti negativno.")]
        public int? RepeatSuppressionSeconds { get; init; }

        /// <summary>Je li pravilo uključeno, zadano true.</summary>
        public bool Enabled { get; init; } = true;
    }
}
