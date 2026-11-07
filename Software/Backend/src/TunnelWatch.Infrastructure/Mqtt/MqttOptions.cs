namespace TunnelWatch.Infrastructure.Mqtt;

/// <summary>
/// Postavke spajanja na MQTT broker, učitane iz sekcije "Mqtt" u appsettings.
/// </summary>
public sealed class MqttOptions
{
    public const string SectionName = "Mqtt";

    /// <summary>Adresa brokera, npr. "localhost".</summary>
    public string Host { get; set; } = "localhost";

    /// <summary>Port brokera, standardno 1883.</summary>
    public int Port { get; set; } = 1883;

    /// <summary>Prefiks topica, odnosno ID uređaja; pretplata ide na "{TopicPrefix}/#".</summary>
    public string TopicPrefix { get; set; } = string.Empty;

    /// <summary>Pauza između pokušaja spajanja kad broker nije dostupan.</summary>
    public int ReconnectDelaySeconds { get; set; } = 5;
}