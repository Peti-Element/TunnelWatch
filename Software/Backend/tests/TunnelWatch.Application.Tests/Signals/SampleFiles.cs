namespace TunnelWatch.Application.Tests.Signals;

/// <summary>
/// Čita snimljene MQTT poruke iz mape samples (kopirane u output testova).
/// Svaki redak je topic, razmak, pa JSON payload.
/// </summary>
internal static class SampleFiles
{
    public const string Truck = "device-truck.txt";
    public const string DrillRig = "device-drill-rig.txt";

    public static IEnumerable<(string Topic, string Payload)> ReadLines(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "samples", fileName);
        foreach (var line in File.ReadLines(path))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var separator = line.IndexOf(' ');
            yield return (line[..separator], line[(separator + 1)..]);
        }
    }
}