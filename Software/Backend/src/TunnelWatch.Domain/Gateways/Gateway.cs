namespace TunnelWatch.Domain.Gateways
{
    public sealed class Gateway
    {
        public Gateway(string id, DateTimeOffset lastMessageAt)
        {
            Id = id;
            LastMessageAt = lastMessageAt;
        }

        /// <summary>ID uređaja, prvi dio MQTT topica, npr. "tdce-01".</summary>
        public string Id { get; private set; }

        /// <summary>Vrijeme najnovije poruke primljene s uređaja.</summary>
        public DateTimeOffset LastMessageAt { get; private set; }

        /// <summary>
        /// Bilježi vrijeme primljene poruke s uređaja, ako je novije od prethodno zabilježenog vremena.
        /// </summary>
        /// <param name="messageTimestamp"></param>
        public void RegisterMessage(DateTimeOffset messageTimestamp)
        {
            if (messageTimestamp > LastMessageAt)
            {
                LastMessageAt = messageTimestamp;
            }
        }
    }
}
