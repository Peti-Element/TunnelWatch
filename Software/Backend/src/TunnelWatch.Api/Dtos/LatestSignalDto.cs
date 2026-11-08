namespace TunnelWatch.Api.Dtos
{
    /// <summary>
    /// Zadnja mjerenja u odgovoru na GET /api/gateways/{gatewayId}/latest.
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Value"></param>
    /// <param name="Unit"></param>
    /// <param name="TimeStamp"></param>
    public sealed record LatestSignalDto(string Name, double Value, string? Unit, DateTimeOffset TimeStamp);
}
