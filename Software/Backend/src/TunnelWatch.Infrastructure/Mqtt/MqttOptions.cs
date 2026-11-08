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

    /// <summary>
    /// MQTT filter pretplate. Zadano "+/+/+": svi topici oblika {uređaj}/{port}/{profil}, za sve uređaje.
    /// </summary>
    public string TopicFilter { get; set; } = "+/+/+";

    /// <summary>Pauza između pokušaja spajanja kad broker nije dostupan.</summary>
    public int ReconnectDelaySeconds { get; set; } = 5;
}