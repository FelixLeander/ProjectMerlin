using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.EntityFrameworkCore;
using ProjectMerlin.Core.Models;

namespace ProjectMerlin.Core.Business;

/// <summary>
/// Handles logic and interactability, related to the <see cref="MonitorConfig"/>.
/// </summary>
public sealed class MonitorManager
{
    /// <summary>
    /// Contains the <see cref="MonitorConfig"/>s which have <see cref="MonitorConfig.IsEnabled"/> set to <see langword="true"/>.
    /// </summary>
    private readonly Dictionary<int, MonitorConfig> _monitoringConfig = new();

    /// <summary>
    /// Adds a new <see cref="Models.MonitorConfig"/> to the database.
    /// </summary>
    /// <param name="monitorConfig"></param>
    /// <returns>A <see langword="bool"/> indicattign success or failure.</returns>
    public bool Add(MonitorConfig monitorConfig)
    {
        try
        {
            Helper.Logger.Verbose("Adding {config}.", nameof(Models.MonitorConfig));
            using var context = new DatabaseManager();
            context.MonitorConfigs.Add(monitorConfig);

            return context.SaveChanges() > 0;
        }
        catch (Exception ex)
        {
            Helper.Logger.Error(ex, "Error adding {config}.", nameof(Models.MonitorConfig));
            return false;
        }
    }

    /// <summary>
    /// Loads the configs from the database to memory.
    /// </summary>
    /// <exception cref="Exception">See <see cref="Exception.InnerException"/>.</exception>
    public async Task LoadMonitorConfigFromDatabase()
    {
        try
        {
            Helper.Logger.Verbose("Loading {config} from database into memory.", nameof(MonitorConfig));

            var monitorConfigs = await DatabaseManager.LoadMonitorConfigAsync();
            _monitoringConfig.Clear();
            if (monitorConfigs.Any(monitorConfig => _monitoringConfig.TryAdd(monitorConfig.ArgbColor, monitorConfig)))
                throw new Exception("Monitor config already exists. Is the database corrupt? Check for duplicates.");
        }
        catch (Exception ex)
        {
            _monitoringConfig.Clear();
            const string error =
                "Could not load saved config from the database. To prevent corruption, internal data has been cleared.";

            Helper.Logger.Error(ex, error);
            throw new Exception(error, ex);
        }
    }
}
