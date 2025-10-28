using System;
using System.Collections.Generic;
using System.Drawing;
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
    private List<MonitorConfig> _monitoringConfig = [];

    /// <summary>
    /// Loads the configs from the database to memory.
    /// </summary>
    /// <exception cref="Exception">See <see cref="Exception.InnerException"/>.</exception>
    public async Task LoadConfigAsync()
    {
        try
        {
            Helper.Logger.Verbose("Loading {config} from database into memory.", nameof(MonitorConfig));

            _monitoringConfig = await DatabaseManager.LoadMonitorConfigAsync();
        }
        catch (Exception ex)
        {
            const string error = "Could not load saved config from the database. To prevent corruption, internal data has been cleared.";
            Helper.Logger.Error(ex, error);
            throw new Exception(error, ex);
        }
    }

    public IReadOnlyList<MonitorConfig> GetMatching(Color color)
    {
        return [.. _monitoringConfig.Where(f => ColorSimilarity(f.Color, color) > f.Threhshold)];

        static double ColorSimilarity(Color c1, Color c2)
        {
            int rDiff = c1.R - c2.R;
            int gDiff = c1.G - c2.G;
            int bDiff = c1.B - c2.B;

            double distance = Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
            double maxDistance = Math.Sqrt(255 * 255 * 3);

            // normalize: 1.0 = identical, 0.0 = opposite
            var result = 1.0 - (distance / maxDistance);
            return result;
        }
    }

    /// <summary>
    /// Adds a new <see cref="MonitorConfig"/> to the database.
    /// </summary>
    /// <param name="monitorConfig"></param>
    /// <returns>A <see langword="bool"/> indicattign success or failure.</returns>
    public bool AddMonitorConfig(params MonitorConfig[] monitorConfig)
    {
        try
        {
            Helper.Logger.Verbose("Adding {count} '{config}'s.", monitorConfig.Length, nameof(MonitorConfig));
            using var context = new DatabaseManager();
            context.MonitorConfigs.AddRange(monitorConfig);

            var changes = context.SaveChanges();
            Helper.Logger.Verbose("Saved {amount} changes.", changes);
            return changes > 0;
        }
        catch (Exception ex)
        {
            Helper.Logger.Error(ex, "Error adding {config}.", nameof(MonitorConfig));
            return false;
        }
    }
}
