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
    }
}
