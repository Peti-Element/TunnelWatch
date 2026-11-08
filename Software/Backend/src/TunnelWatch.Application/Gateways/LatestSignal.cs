namespace TunnelWatch.Application.Gateways
{
    public sealed record LatestSignal(string Name, double Value, string? Unit, DateTimeOffset Timestamp);
}
