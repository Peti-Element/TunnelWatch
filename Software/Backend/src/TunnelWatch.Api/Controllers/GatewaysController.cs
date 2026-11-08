using Microsoft.AspNetCore.Mvc;
using TunnelWatch.Api.Dtos;
using TunnelWatch.Application.Gateways;

namespace TunnelWatch.Api.Controllers;

/// <summary>
/// REST endpointi za uređaje i zadnje vrijednosti njihovih signala.
/// </summary>
[ApiController]
[Route("api/gateways")]
[Produces("application/json")]
public sealed class GatewaysController(IGatewayQueries queries) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Popis uređaja")]
    [EndpointDescription("Svi uređaji koji su slali podatke, s vremenom zadnje poruke, sortirani po ID-u.")]
    [ProducesResponseType<IReadOnlyList<GatewayDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<GatewayDto>>> GetGateways(CancellationToken cancellationToken)
    {
        var gateways = await queries.GetGatewaysAsync(cancellationToken);

        return Ok(gateways.Select(g => new GatewayDto(g.Id, g.LastMessageAt)).ToList());
    }

    [HttpGet("{id}/latest")]
    [EndpointSummary("Zadnje vrijednosti signala uređaja")]
    [EndpointDescription("Zadnja vrijednost, jedinica i vrijeme za svaki signal uređaja, sortirano po nazivu signala. 404 ako za uređaj nema podataka.")]
    [ProducesResponseType<IReadOnlyList<LatestSignalDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<LatestSignalDto>>> GetLatest(string id, CancellationToken cancellationToken)
    {
        var signals = await queries.GetLatestSignalsAsync(id, cancellationToken);
        if (signals.Count == 0)
        {
            return Problem(
                title: "Nema podataka za uređaj",
                detail: $"Za uređaj '{id}' nema spremljenih signala.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(signals.Select(s => new LatestSignalDto(s.Name, s.Value, s.Unit, s.TimeStamp)).ToList());
    }
}