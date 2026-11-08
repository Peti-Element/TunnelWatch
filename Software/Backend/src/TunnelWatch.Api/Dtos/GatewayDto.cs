namespace TunnelWatch.Api.Dtos
{
    /// <summary>
    /// Uređaj u odgovoru na GET /api/gateways.
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="LastMessageAt"></param>
    public sealed record GatewayDto(string Id, DateTimeOffset LastMessageAt);
}
