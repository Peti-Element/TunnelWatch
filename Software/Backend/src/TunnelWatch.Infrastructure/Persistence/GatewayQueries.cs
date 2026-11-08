using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TunnelWatch.Application.Gateways;

namespace TunnelWatch.Infrastructure.Persistence
{
    public sealed class GatewayQueries(TunnelWatchDbContext db) : IGatewayQueries
    {
        public async Task<IReadOnlyList<GatewaySummary>> GetGatewaysAsync(CancellationToken cancellationToken)
        {
            return await db.Gateways.AsNoTracking().OrderBy(g => g.Id).Select(g => new GatewaySummary(g.Id, g.LastMessageAt)).ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<LatestSignal>> GetLatestSignalsAsync(string gatewayId, CancellationToken cancellationToken)
        {
            return await db.LatestSignalValues.AsNoTracking().Where(s => s.GatewayId == gatewayId).OrderBy(s => s.SignalName).Select(s => new LatestSignal(s.SignalName, s.Value, s.Unit, s.Timestamp)).ToListAsync(cancellationToken);
        }
    }
}
