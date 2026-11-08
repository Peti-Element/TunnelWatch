namespace TunnelWatch.Domain.Signals;

/// <summary>
/// Jedno očitanje jednog signala stroja, onako kako ga je CAN App dekodirao i poslao preko MQTT-a.
/// </summary>
/// <param name="GatewayId">ID uređaja (gatewaya), prvi dio topica, npr. "tdce-01".</param>
/// <param name="Port">CAN port s kojeg je poruka pročitana, npr. "can0".</param>
/// <param name="Profile">Profil dekodiranja, npr. "j1939-default".</param>
/// <param name="Name">Naziv signala, npr. "engine_speed".</param>
/// <param name="Value">Vrijednost signala uvijek double jer isti signal zna doći kao cijeli ili decimalni broj.</param>
/// <param name="Unit">Mjerna jedinica null kod statusnih signala (npr. "parking_brake_switch").</param>
/// <param name="Timestamp">Vrijeme očitanja u UTC-u.</param>
public sealed record MachineSignal(
    string GatewayId,
    string Port,
    string Profile,
    string Name,
    double Value,
    string? Unit,
    DateTimeOffset Timestamp);