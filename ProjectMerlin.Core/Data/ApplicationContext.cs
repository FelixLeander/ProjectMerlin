using Microsoft.EntityFrameworkCore;
using ProjectMerlin.Core.Models;

namespace ProjectMerlin.Core.Data;

internal sealed class ApplicationContext : DbContext
{
    internal DbSet<MonitorConfig> MonitorConfigs => Set<MonitorConfig>();
    internal DbSet<TriggerAction> TriggerActions => Set<TriggerAction>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source=Database.db");
}
