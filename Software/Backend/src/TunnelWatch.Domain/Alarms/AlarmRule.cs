using System;
using System.Collections.Generic;
using System.Text;

namespace TunnelWatch.Domain.Alarms
{
    /// <summary>
    /// Pravilo alarma, npr. "engine_coolant_temperature &gt; 95 dulje od 10 s, histereza 3, kritično, ponavljanje nakon 300 s".
    /// Konstruktor i Update odbijaju neispravne vrijednosti, pa pravilo u memoriji i u bazi uvijek ima smisla.
    /// </summary>
    public sealed class AlarmRule
    {
        public AlarmRule(string gatewayId, string signalName, ComparisonOperator @operator, double threshold, int durationSeconds, double hysteresis, AlarmSeverity severity, int repeatSuppressionSeconds, bool enabled)
        {
            GatewayId = string.Empty;
            SignalName = string.Empty;
            Update(gatewayId, signalName, @operator, threshold, durationSeconds, hysteresis, severity, repeatSuppressionSeconds, enabled);
        }

        /// <summary>ID pravila, dodjeljuje ga baza pri spremanju.</summary>
        public int Id { get; private set; }

        /// <summary>Uređaj na koji se pravilo odnosi, npr. "tdce-01".</summary>
        public string GatewayId { get; private set; }

        /// <summary>Signal koji se prati, npr. "engine_coolant_temperature".</summary>
        public string SignalName { get; private set; }

        /// <summary>Usporedba vrijednosti signala s pragom.</summary>
        public ComparisonOperator Operator { get; private set; }

        /// <summary>Prag u jedinici signala, npr. 95 (°C).</summary>
        public double Threshold { get; private set; }

        /// <summary>Koliko dugo (u sekundama) uvjet mora vrijediti prije alarma; 0 znači odmah.</summary>
        public int DurationSeconds { get; private set; }

        /// <summary>Koliko se vrijednost mora vratiti ispod (ili iznad) praga da se alarm smatra završenim, u jedinici signala.</summary>
        public double Hysteresis { get; private set; }

        /// <summary>Razina ozbiljnosti alarma.</summary>
        public AlarmSeverity Severity { get; private set; }

        /// <summary>Najkraći razmak (u sekundama) između dva alarma istog pravila.</summary>
        public int RepeatSuppressionSeconds { get; private set; }

        /// <summary>Isključeno pravilo ostaje spremljeno, ali ne stvara alarme.</summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Mijenja sva polja pravila odjednom. Baca ArgumentException za neispravne vrijednosti, i tada se ništa ne mijenja.
        /// </summary>
        public void Update(string gatewayId, string signalName, ComparisonOperator @operator, double threshold, int durationSeconds, double hysteresis, AlarmSeverity severity, int repeatSuppressionSeconds, bool enabled)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(gatewayId);
            ArgumentException.ThrowIfNullOrWhiteSpace(signalName);

            if (!Enum.IsDefined(@operator))
            {
                throw new ArgumentOutOfRangeException(nameof(@operator), @operator, "Nepoznat operator.");
            }

            if (!Enum.IsDefined(severity))
            {
                throw new ArgumentOutOfRangeException(nameof(severity), severity, "Nepoznata razina ozbiljnosti.");
            }

            if (!double.IsFinite(threshold))
            {
                throw new ArgumentOutOfRangeException(nameof(threshold), threshold, "Prag mora biti broj.");
            }

            ArgumentOutOfRangeException.ThrowIfNegative(durationSeconds);
            ArgumentOutOfRangeException.ThrowIfNegative(repeatSuppressionSeconds);

            if (!double.IsFinite(hysteresis) || hysteresis < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(hysteresis), hysteresis, "Histereza ne smije biti negativna.");
            }

            GatewayId = gatewayId.Trim();
            SignalName = signalName.Trim();
            Operator = @operator;
            Threshold = threshold;
            DurationSeconds = durationSeconds;
            Hysteresis = hysteresis;
            Severity = severity;
            RepeatSuppressionSeconds = repeatSuppressionSeconds;
            Enabled = enabled;
        }
    }
}
