using Microsoft.EntityFrameworkCore;
using ProjectMerlin.Core.Models;

namespace ProjectMerlin.Core.Business;

/// <summary>
/// Handles persistant data, uses sqlite.
/// </summary>
public sealed class DatabaseManager : DbContext
{
    public DbSet<MonitorConfig> MonitorConfigs => Set<MonitorConfig>();
    public DbSet<TriggerAction> TriggerActions => Set<TriggerAction>();


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source=Database.db");

    public static void Initialize(bool reset = false)
    {
        using var context = new DatabaseManager();
        if (reset)
            context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    public static async Task<List<MonitorConfig>> LoadEnaabledMonitorConfigAsync()
    {
        await using var context = new DatabaseManager();
        return await context.MonitorConfigs
            .AsNoTracking()
            .Where(w => w.IsEnabled)
            .Include(i => i.TriggerActions)
            .ToListAsync();
    }
}
