using TunnelWatch.Domain.Alarms;

namespace TunnelWatch.Application.Alarms
{
    public interface IAlarmRuleRepository
    {
        /// <summary>Sva pravila, sortirana po ID-u.</summary>
        Task<IReadOnlyList<AlarmRule>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>Pravilo s tim ID-om ili null, vraćeno pravilo se može mijenjati pa spremiti sa SaveChangesAsync.</summary>
        Task<AlarmRule?> FindAsync(int id, CancellationToken cancellationToken);

        /// <summary>Dodaje novo pravilo i odmah ga sprema, nakon toga pravilo ima ID.</summary>
        Task AddAsync(AlarmRule rule, CancellationToken cancellationToken);

        /// <summary>Briše pravilo i odmah sprema promjenu.</summary>
        Task RemoveAsync(AlarmRule rule, CancellationToken cancellationToken);

        /// <summary>Sprema izmjene pravila dohvaćenog s FindAsync.</summary>
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
