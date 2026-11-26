using Microsoft.EntityFrameworkCore;
using TunnelWatch.Application.Alarms;
using TunnelWatch.Domain.Alarms;

namespace TunnelWatch.Infrastructure.Persistence
{
    /// <summary>
    /// EF Core implementacija spremišta pravila alarma.
    /// </summary>
    public sealed class AlarmRuleRepository(TunnelWatchDbContext db) : IAlarmRuleRepository
    {
        public async Task<IReadOnlyList<AlarmRule>> GetAllAsync(CancellationToken cancellationToken) =>
            await db.AlarmRules.AsNoTracking().OrderBy(r => r.Id).ToListAsync(cancellationToken);

        public async Task<AlarmRule?> FindAsync(int id, CancellationToken cancellationToken) =>
            await db.AlarmRules.FindAsync([id], cancellationToken);

        public async Task AddAsync(AlarmRule rule, CancellationToken cancellationToken)
        {
            db.AlarmRules.Add(rule);
            await db.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveAsync(AlarmRule rule, CancellationToken cancellationToken)
        {
            db.AlarmRules.Remove(rule);
            await db.SaveChangesAsync(cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
    }
}
