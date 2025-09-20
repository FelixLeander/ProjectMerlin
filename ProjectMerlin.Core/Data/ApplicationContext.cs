using Microsoft.EntityFrameworkCore;
using ProjectMerlin.Core.Models;

namespace ProjectMerlin.Core.Data;

public sealed class ApplicationContext : DbContext
{
    public DbSet<Config> Configurattions => Set<Config>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source=Database.db");
}
