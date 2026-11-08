namespace TunnelWatch.Domain.Signals
{
    /// <summary>
    /// Povijest signala u smanjenoj rezoluciji, po minutama, sa sažetkom svih vrijednosti u toj minuti.
    /// </summary>
    public sealed class SignalMinuteHistory
    {
        public SignalMinuteHistory(string gatewayId, string signalName, DateTimeOffset minute, double minimum, double maximum, double average, int sampleCount, string? unit)
        {
            GatewayId = gatewayId;
            SignalName = signalName;
            Minute = minute;
            Minimum = minimum;
            Maximum = maximum;
            Average = average;
            SampleCount = sampleCount;
            Unit = unit;
        }
        /// <summary>ID uređaja, prvi dio MQTT topica.</summary>
        public string GatewayId { get; private set; }
        /// <summary>Naziv signala iz poruke</summary>
        public string SignalName { get; private set; }
        /// <summary>Vrijeme početka minute u UTC-u.</summary>
        public DateTimeOffset Minute { get; private set; }
        /// <summary>Najmanja izmjerena vrijednost signala u toj minuti.</summary>
        public double Minimum { get; private set; }
        /// <summary>Najveća izmjerena vrijednost signala u toj minuti.</summary>
        public double Maximum { get; private set; }
        /// <summary>Prosječna izmjerena vrijednost signala u toj minuti.</summary>
        public double Average { get; private set; }
        /// <summary>Broj očitanja signala u toj minuti.</summary>
        public int SampleCount { get; private set; }

        public string? Unit { get; private set; }
    }
}
