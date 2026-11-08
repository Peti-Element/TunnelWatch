namespace TunnelWatch.Domain.Signals
{
    /// <summary>
    /// Zadnja poznata vrijednost jednog signala jednog uređaja.
    /// </summary>
    public sealed class LatestSignalValue
    {
        public LatestSignalValue(string gatewayId, string signalName, string? unit, DateTimeOffset timestamp, double value)
        {
            GatewayId = gatewayId;
            SignalName = signalName;
            Unit = unit;
            Timestamp = timestamp;
            Value = value;
        }
        /// <summary>ID uređaja, prvi dio MQTT topica.</summary>
        public string GatewayId { get; private set; }
        /// <summary>ID signala, drugi dio MQTT topica.</summary>
        public string SignalName { get; private set; }
        /// <summary>Jedinica mjere signala.</summary>
        public string? Unit { get; private set; }
        /// <summary>Vrijeme kada je signal izmjeren.</summary>
        public DateTimeOffset Timestamp { get; private set; }
        /// <summary>Vrijednost signala.</summary>
        public double Value { get; private set; }

        /// <summary>
        /// Pokušava ažurirati zadnju poznatu vrijednost signala, ako je proslijeđeno vrijeme novije od prethodno zabilježenog vremena.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="timestamp"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool UpdateIfNotOlder(string? unit, DateTimeOffset timestamp, double value)
        {
            if (timestamp >= Timestamp)
            {
                Unit = unit;
                Timestamp = timestamp;
                Value = value;
                return true;
            }
            return false;
        }
    }
}
