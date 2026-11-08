using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TunnelWatch.Domain.Signals;

namespace TunnelWatch.Application.Signals;

/// <summary>
/// Parser payloada CAN Appa. Nikad ne baca iznimku prema pozivatelju:
/// neispravne poruke zapisuje u log i preskače, a duplikate (isti signal i timestamp) uklanja.
/// </summary>
public sealed class MachineMessageParser(ILogger<MachineMessageParser> logger) : IMachineMessageParser
{
    // Raspon milisekundi koji DateTimeOffset može prikazati (0001-01-01 do 9999-12-31).
    private static readonly long MinUnixMilliseconds = DateTimeOffset.MinValue.ToUnixTimeMilliseconds();
    private static readonly long MaxUnixMilliseconds = DateTimeOffset.MaxValue.ToUnixTimeMilliseconds();

    public IReadOnlyList<MachineSignal> Parse(string topic, string payload) =>
        Parse(topic, Encoding.UTF8.GetBytes(payload ?? string.Empty));

    public IReadOnlyList<MachineSignal> Parse(string topic, ReadOnlyMemory<byte> payload)
    {
        if (!TryParseTopic(topic, out var gatewayId, out var port, out var profile))
        {
            logger.LogWarning("Preskačem poruku: topic '{Topic}' nije oblika {{prefix}}/{{port}}/{{profile}}", topic);
            return [];
        }

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(payload);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Preskačem poruku na topicu '{Topic}': payload nije ispravan JSON", topic);
            return [];
        }

        using (document)
        {
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object
                || !root.TryGetProperty("messages", out var messages)
                || messages.ValueKind != JsonValueKind.Array)
            {
                logger.LogWarning("Preskačem poruku na topicu '{Topic}': nedostaje niz 'messages'", topic);
                return [];
            }

            var result = new List<MachineSignal>();
            // Ključ za uklanjanje duplikata; uređaj je isti za cijeli payload, pa je dovoljan naziv + vrijeme.
            var seen = new HashSet<(string Name, DateTimeOffset Timestamp)>();

            foreach (var message in messages.EnumerateArray())
            {
                if (!TryReadTimestamp(message, out var timestamp))
                {
                    logger.LogWarning("Preskačem CAN poruku na topicu '{Topic}': nedostaje ili je neispravan 'timestamp'", topic);
                    continue;
                }

                if (!message.TryGetProperty("signals", out var signals) || signals.ValueKind != JsonValueKind.Object)
                {
                    logger.LogWarning("Preskačem CAN poruku na topicu '{Topic}': nedostaje objekt 'signals'", topic);
                    continue;
                }

                foreach (var signal in signals.EnumerateObject())
                {
                    if (!TryReadValue(signal.Value, out var value))
                    {
                        logger.LogWarning("Preskačem signal '{Signal}' na topicu '{Topic}': 'value' nedostaje ili nije broj", signal.Name, topic);
                        continue;
                    }

                    if (!seen.Add((signal.Name, timestamp)))
                    {
                        continue; // duplikat iste poruke
                    }

                    result.Add(new MachineSignal(gatewayId, port, profile, signal.Name, value, ReadUnit(signal.Value), timestamp));
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Topic mora imati točno tri neprazna dijela: {prefix}/{port}/{profile}.
    /// </summary>
    private static bool TryParseTopic(string? topic, out string gatewayId, out string port, out string profile)
    {
        gatewayId = port = profile = string.Empty;
        var parts = (topic ?? string.Empty).Split('/');
        if (parts.Length != 3 || parts.Any(string.IsNullOrWhiteSpace))
        {
            return false;
        }

        (gatewayId, port, profile) = (parts[0], parts[1], parts[2]);
        return true;
    }

    /// <summary>
    /// Čita 'timestamp' (Unix vrijeme u milisekundama) i pretvara ga u UTC DateTimeOffset.
    /// </summary>
    private static bool TryReadTimestamp(JsonElement message, out DateTimeOffset timestamp)
    {
        timestamp = default;
        if (message.ValueKind != JsonValueKind.Object
            || !message.TryGetProperty("timestamp", out var element)
            || element.ValueKind != JsonValueKind.Number
            || !element.TryGetInt64(out var milliseconds)
            || milliseconds < MinUnixMilliseconds
            || milliseconds > MaxUnixMilliseconds)
        {
            return false;
        }

        timestamp = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        return true;
    }

    /// <summary>
    /// Čita 'value' kao double, bez obzira je li u JSON-u zapisan kao cijeli ili decimalni broj.
    /// </summary>
    private static bool TryReadValue(JsonElement signal, out double value)
    {
        value = 0;
        return signal.ValueKind == JsonValueKind.Object
            && signal.TryGetProperty("value", out var element)
            && element.ValueKind == JsonValueKind.Number
            && element.TryGetDouble(out value);
    }

    /// <summary>
    /// 'unit' je opcionalan (statusni signali ga nemaju), pa vraća null ako ga nema.
    /// </summary>
    private static string? ReadUnit(JsonElement signal) =>
        signal.TryGetProperty("unit", out var element) && element.ValueKind == JsonValueKind.String
            ? element.GetString()
            : null;
}