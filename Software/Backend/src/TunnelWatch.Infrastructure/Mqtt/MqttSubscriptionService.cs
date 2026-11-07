using System.Buffers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using TunnelWatch.Application.Signals;

namespace TunnelWatch.Infrastructure.Mqtt;

/// <summary>
/// Pozadinski servis koji se spaja na MQTT broker, pretplaćuje na "{TopicPrefix}/#"
/// i svaku primljenu poruku šalje parseru. Ako broker nije dostupan ili veza padne,
/// periodično se pokušava ponovno spojiti, a aplikacija nastavlja raditi.
/// </summary>
public sealed class MqttSubscriptionService(
    IOptions<MqttOptions> options,
    IServiceScopeFactory scopeFactory,
    ILogger<MqttSubscriptionService> logger) : BackgroundService
{
    private readonly MqttOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new MqttClientFactory();
        using var client = factory.CreateMqttClient();

        var clientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(_options.Host, _options.Port)
            .WithClientId($"tunnelwatch-backend-{Guid.NewGuid():N}")
            .WithCleanSession()
            .WithTimeout(TimeSpan.FromSeconds(5))
            .Build();

        var topicFilter = $"{_options.TopicPrefix}/#";
        var subscribeOptions = factory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(topicFilter)
            .Build();

        client.ApplicationMessageReceivedAsync += e =>
        {
            HandleMessage(e.ApplicationMessage);
            return Task.CompletedTask;
        };

        client.DisconnectedAsync += e =>
        {
            if (e.ClientWasConnected)
            {
                logger.LogWarning("Veza s MQTT brokerom {Host}:{Port} je prekinuta", _options.Host, _options.Port);
            }

            return Task.CompletedTask;
        };

        var reconnectDelay = TimeSpan.FromSeconds(Math.Max(1, _options.ReconnectDelaySeconds));

        // Petlja nadzora veze: svakih nekoliko sekundi provjeri vezu i po potrebi se ponovno spoji.
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!await client.TryPingAsync(stoppingToken))
                {
                    await client.ConnectAsync(clientOptions, stoppingToken);
                    // Clean session: broker ne pamti pretplatu, pa se pretplaćujemo nakon svakog spajanja.
                    await client.SubscribeAsync(subscribeOptions, stoppingToken);
                    logger.LogInformation("Spojen na MQTT broker {Host}:{Port}, pretplata na '{Topic}'", _options.Host, _options.Port, topicFilter);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning("MQTT broker {Host}:{Port} nije dostupan ({Error}), novi pokušaj za {Delay} s",
                    _options.Host, _options.Port, ex.Message, reconnectDelay.TotalSeconds);
            }

            try
            {
                await Task.Delay(reconnectDelay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        if (client.IsConnected)
        {
            await client.DisconnectAsync(new MqttClientDisconnectOptionsBuilder().Build(), CancellationToken.None);
        }
    }

    /// <summary>
    /// Obrađuje jednu poruku u vlastitom DI scopeu, greška se zapisuje u log i ne zaustavlja pretplatu.
    /// </summary>
    private void HandleMessage(MqttApplicationMessage message)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var parser = scope.ServiceProvider.GetRequiredService<IMachineMessageParser>();

            var signals = parser.Parse(message.Topic, message.Payload.ToArray());
            if (signals.Count == 0)
            {
                return; // parser je već zapisao upozorenje
            }

            // Dok spremanje ne postoji, samo bilježimo koliko je signala parsirano.
            logger.LogInformation("Primljeno {Count} signala s topica '{Topic}'", signals.Count, message.Topic);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Greška pri obradi MQTT poruke s topica '{Topic}'", message.Topic);
        }
    }
}