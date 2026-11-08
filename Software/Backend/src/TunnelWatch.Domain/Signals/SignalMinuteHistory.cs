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

        /// <summary>
        /// Zaokruživanje vremena na početak minute u UTC-u..
        /// </summary>
        /// <param name="timeOffset"></param>
        /// <returns></returns>
        public static DateTimeOffset ToMinute(DateTimeOffset timeOffset)
        {
            var utc = timeOffset.ToUniversalTime();
            return new DateTimeOffset(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, 0, TimeSpan.Zero);
        }

        /// <summary>
        /// Stvaranje nove instance povijesti signala iz pojedinačnog očitanja signala.
        /// </summary>
        /// <param name="gatewayId"></param>
        /// <param name="signalName"></param>
        /// <param name="timestamp"></param>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static SignalMinuteHistory CreateFromSignal(string gatewayId, string signalName, DateTimeOffset timestamp, double value, string? unit)
        {
            var minute = ToMinute(timestamp);
            return new SignalMinuteHistory(gatewayId, signalName, minute, value, value, value, 1, unit);
        }

        /// <summary>
        /// Dodavanje novog očitanja signala u postojeću povijest signala, ažuriranje prosjeka, minimuma, maksimuma i broja uzoraka.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        public void AddSample(double value, string? unit)
        {
            SampleCount++;
            Average += (value - Average) / SampleCount;
            Minimum = Math.Min(Minimum, value);
            Maximum = Math.Max(Maximum, value);
            Unit = unit ?? Unit;
        }
    }
}
