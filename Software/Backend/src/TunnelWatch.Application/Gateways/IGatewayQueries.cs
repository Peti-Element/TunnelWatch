using System;
using System.Collections.Generic;
using System.Text;

namespace TunnelWatch.Application.Gateways
{
    public interface IGatewayQueries
    {
        Task<IReadOnlyList<GatewaySummary>> GetGatewaysAsync(CancellationToken cancellationToken);

        Task<IReadOnlyList<LatestSignal>> GetLatestSignalsAsync(string gatewayId, CancellationToken cancellationToken);
    }
}
