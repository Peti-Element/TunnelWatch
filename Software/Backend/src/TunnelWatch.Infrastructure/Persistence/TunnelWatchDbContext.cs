using Microsoft.EntityFrameworkCore;
using TunnelWatch.Domain.Gateways;
using TunnelWatch.Domain.Signals;

namespace TunnelWatch.Infrastructure.Persistence
{
    /// <summary>
    /// EF Core pristup PostgreSQL bazi podataka za pohranu podataka o signalima i uređajima i povijesti po minuti.
    /// </summary>
    public sealed class TunnelWatchDbContext(DbContextOptions<TunnelWatchDbContext> options) : DbContext(options)
    {
        public DbSet<Gateway> Gateways => Set<Gateway>();
        public DbSet<LatestSignalValue> LatestSignalValues => Set<LatestSignalValue>();
        public DbSet<SignalMinuteHistory> SignalMinuteHistories => Set<SignalMinuteHistory>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TunnelWatchDbContext).Assembly);
        }
    }
}
