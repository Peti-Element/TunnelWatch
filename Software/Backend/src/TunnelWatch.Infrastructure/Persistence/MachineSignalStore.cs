using System;
using System.Collections.Generic;
using System.Text;
using TunnelWatch.Application.Signals;
using TunnelWatch.Domain.Gateways;
using TunnelWatch.Domain.Signals;

namespace TunnelWatch.Infrastructure.Persistence
{
    public sealed class MachineSignalStore(TunnelWatchDbContext db) : IMachineSignalStore
    {
        public async Task StoreAsync(IReadOnlyList<MachineSignal> signals, CancellationToken cancellationToken)
        {
            if (signals.Count == 0)
                return;

            foreach(var gatewaySignals in signals.GroupBy(s => s.GatewayId))
            {
                await ApplyGatewaySignalsAsync(gatewaySignals.Key, gatewaySignals.ToList(), cancellationToken);
            }

            await db.SaveChangesAsync(cancellationToken);
        }

        private async Task ApplyGatewaySignalsAsync(string gatewayId, IReadOnlyList<MachineSignal> signals, CancellationToken cancellationToken)
        {
            var newestTimestamp = signals.Max(s => s.Timestamp);
            var gateway = await db.Gateways.FindAsync(new object[] { gatewayId }, cancellationToken);
            if (gateway == null)
            {
                db.Gateways.Add(new Gateway(gatewayId, newestTimestamp));
            }
            else
            {
                gateway.RegisterMessage(newestTimestamp);
            }

            foreach (var signal in signals)
            {
                var latestSignalValue = await db.LatestSignalValues.FindAsync(new object[] { gatewayId, signal.Name }, cancellationToken);
                if (latestSignalValue == null)
                {
                    latestSignalValue = new LatestSignalValue(gatewayId, signal.Name, signal.Unit, signal.Timestamp, signal.Value);
                    db.LatestSignalValues.Add(latestSignalValue);
                }
                else
                {
                    latestSignalValue.UpdateIfNotOlder(signal.Unit, signal.Timestamp, signal.Value);
                }

                var minute = SignalMinuteHistory.ToMinute(signal.Timestamp);
                var history = await db.SignalMinuteHistories.FindAsync(new object[] { gatewayId, signal.Name, minute }, cancellationToken);
                if (history == null)
                {
                    history = SignalMinuteHistory.CreateFromSignal(gatewayId, signal.Name, signal.Timestamp, signal.Value, signal.Unit);
                    db.SignalMinuteHistories.Add(history);
                }
                else
                {
                    history.AddSample(signal.Value, signal.Unit);
                }
            }
        }
    }
}
