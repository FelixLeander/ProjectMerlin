using Microsoft.EntityFrameworkCore;
using ProjectMerlin.Core.Models;

namespace ProjectMerlin.Core.Business;

/// <summary>
/// Handles persistant data, uses sqlite.
/// </summary>
internal sealed class DatabaseManager : DbContext
{
    internal DbSet<MonitorConfig> MonitorConfigs => Set<MonitorConfig>();
    internal DbSet<TriggerAction> TriggerActions => Set<TriggerAction>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source=Database.db");

    internal static void Initialize(bool reset = false)
    {
        using var context = new DatabaseManager();
        if (reset)
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
