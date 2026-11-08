using TunnelWatch.Domain.Signals;

namespace TunnelWatch.Application.Signals;

/// <summary>
/// Pretvara jednu MQTT poruku CAN Appa (topic + JSON payload) u listu signala.
/// </summary>
public interface IMachineMessageParser
{
    /// <summary>
    /// Parsira payload zadan kao tekst. Za neispravan topic ili JSON vraća praznu listu.
    /// </summary>
    IReadOnlyList<MachineSignal> Parse(string topic, string payload);

    /// <summary>
    /// Parsira payload zadan kao UTF-8 bajtovi (onako kako stiže s MQTT-a).
    /// </summary>
    IReadOnlyList<MachineSignal> Parse(string topic, ReadOnlyMemory<byte> payload);
}