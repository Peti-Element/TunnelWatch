using Microsoft.AspNetCore.Mvc;
using TunnelWatch.Api.Dtos;
using TunnelWatch.Application.Alarms;
using TunnelWatch.Domain.Alarms;

namespace TunnelWatch.Api.Controllers
{
    /// <summary>
    /// REST endpointi za pravila alarma: popis, dohvat, dodavanje, izmjena i brisanje.
    /// </summary>
    [ApiController]
    [Route("api/rules")]
    [Produces("application/json")]
    public sealed class RulesController(IAlarmRuleRepository rules) : ControllerBase
    {
        [HttpGet]
        [EndpointSummary("Popis pravila alarma")]
        [EndpointDescription("Sva pravila alarma, sortirana po ID-u.")]
        [ProducesResponseType<IReadOnlyList<AlarmRuleDto>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<AlarmRuleDto>>> GetRules(CancellationToken cancellationToken)
        {
            var all = await rules.GetAllAsync(cancellationToken);

            return Ok(all.Select(ToDto).ToList());
        }

        [HttpGet("{id:int}")]
        [EndpointSummary("Jedno pravilo alarma")]
        [ProducesResponseType<AlarmRuleDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AlarmRuleDto>> GetRule(int id, CancellationToken cancellationToken)
        {
            var rule = await rules.FindAsync(id, cancellationToken);

            return rule is null ? RuleNotFound(id) : Ok(ToDto(rule));
        }

        [HttpPost]
        [EndpointSummary("Dodavanje pravila alarma")]
        [EndpointDescription("Sprema novo pravilo. 400 ako je neko polje prazno ili neispravno (npr. nepoznat operator, negativno trajanje).")]
        [ProducesResponseType<AlarmRuleDto>(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AlarmRuleDto>> CreateRule(AlarmRuleRequest request, CancellationToken cancellationToken)
        {
            var rule = new AlarmRule(
                request.GatewayId,
                request.SignalName,
                ToDomain(request.Operator!.Value),
                request.Threshold!.Value,
                request.DurationSeconds!.Value,
                request.Hysteresis!.Value,
                request.Severity!.Value,
                request.RepeatSuppressionSeconds!.Value,
                request.Enabled);

            await rules.AddAsync(rule, cancellationToken);

            return CreatedAtAction(nameof(GetRule), new { id = rule.Id }, ToDto(rule));
        }

        [HttpPut("{id:int}")]
        [EndpointSummary("Izmjena pravila alarma")]
        [EndpointDescription("Zamjenjuje sva polja pravila, uključujući uključeno/isključeno. 400 za neispravno pravilo, 404 ako pravilo ne postoji.")]
        [ProducesResponseType<AlarmRuleDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AlarmRuleDto>> UpdateRule(int id, AlarmRuleRequest request, CancellationToken cancellationToken)
        {
            var rule = await rules.FindAsync(id, cancellationToken);
            if (rule is null)
            {
                return RuleNotFound(id);
            }

            rule.Update(
                request.GatewayId,
                request.SignalName,
                ToDomain(request.Operator!.Value),
                request.Threshold!.Value,
                request.DurationSeconds!.Value,
                request.Hysteresis!.Value,
                request.Severity!.Value,
                request.RepeatSuppressionSeconds!.Value,
                request.Enabled);
            await rules.SaveChangesAsync(cancellationToken);

            return Ok(ToDto(rule));
        }

        [HttpDelete("{id:int}")]
        [EndpointSummary("Brisanje pravila alarma")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRule(int id, CancellationToken cancellationToken)
        {
            var rule = await rules.FindAsync(id, cancellationToken);
            if (rule is null)
            {
                return RuleNotFound(id);
            }

            await rules.RemoveAsync(rule, cancellationToken);

            return NoContent();
        }

        private ObjectResult RuleNotFound(int id) =>
            Problem(title: "Pravilo ne postoji", detail: $"Pravilo s ID-om {id} ne postoji.", statusCode: StatusCodes.Status404NotFound);

        private static AlarmRuleDto ToDto(AlarmRule rule) =>
            new(
                rule.Id,
                rule.GatewayId,
                rule.SignalName,
                ToApi(rule.Operator),
                rule.Threshold,
                rule.DurationSeconds,
                rule.Hysteresis,
                rule.Severity,
                rule.RepeatSuppressionSeconds,
                rule.Enabled);

        private static ComparisonOperator ToDomain(RuleOperator value) =>
            value switch
            {
                RuleOperator.GreaterThan => ComparisonOperator.GreaterThan,
                RuleOperator.GreaterThanOrEqual => ComparisonOperator.GreaterThanOrEqual,
                RuleOperator.LessThan => ComparisonOperator.LessThan,
                RuleOperator.LessThanOrEqual => ComparisonOperator.LessThanOrEqual,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Nepoznat operator."),
            };

        private static RuleOperator ToApi(ComparisonOperator value) =>
            value switch
            {
                ComparisonOperator.GreaterThan => RuleOperator.GreaterThan,
                ComparisonOperator.GreaterThanOrEqual => RuleOperator.GreaterThanOrEqual,
                ComparisonOperator.LessThan => RuleOperator.LessThan,
                ComparisonOperator.LessThanOrEqual => RuleOperator.LessThanOrEqual,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Nepoznat operator."),
            };
    }
}
