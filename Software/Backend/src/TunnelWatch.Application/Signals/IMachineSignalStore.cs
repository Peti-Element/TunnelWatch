using TunnelWatch.Domain.Signals;

namespace TunnelWatch.Application.Signals
{
    public interface IMachineSignalStore
    {
        /// <summary>
        /// Sprema listu signala u bazu podataka. Ako je signal stariji od već pohranjenog, neće se prepisati.
        /// </summary>
        /// <param name="signals"></param>
        /// <param name="cancellationToken"></param>
        Task StoreAsync(IReadOnlyList<MachineSignal> signals, CancellationToken cancellationToken);
    }
}
