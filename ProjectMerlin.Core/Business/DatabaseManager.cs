using Microsoft.EntityFrameworkCore;
using ProjectMerlin.Core.Models;

namespace ProjectMerlin.Core.Business;

/// <summary>
/// Handles persistant data, uses sqlite.
/// </summary>
internal sealed class DatabaseManager : DbContext
{
    internal DatabaseManager() { }
    internal DbSet<MonitorConfig> MonitorConfigs => Set<MonitorConfig>();
    internal DbSet<TriggerAction> TriggerActions => Set<TriggerAction>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source=Database.db");

    internal static void Reset()
    {
        using var context = new DatabaseManager();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    internal static async Task<List<MonitorConfig>> LoadMonitorConfigAsync()
    {
        await using var context = new DatabaseManager();
        return await context.MonitorConfigs
            .AsNoTracking()
            .Where(w => w.IsEnabled)
            .Include(i => i.TriggerActions)
            .ToListAsync();
    }
}
