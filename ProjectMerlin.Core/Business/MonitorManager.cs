using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.EntityFrameworkCore;
using ProjectMerlin.Core.Data;
using ProjectMerlin.Core.Models;
using Serilog;

namespace ProjectMerlin.Core.Business;
public static class MonitorManager
{
    private static readonly Dictionary<Guid, MonitorConfig> _monitorConfig = [];

    /// <summary>
    /// Adds a new <see cref="MonitorConfig"/> to the databaase.
    /// </summary>
    /// <param name="monitorConfig"></param>
    /// <returns>A <see langword="bool"/> indicattign sucess or failure.</returns>
    public static bool Add(MonitorConfig monitorConfig)
    {
        try
        {
            Helper.Logger.Verbose("Adding {config}.", nameof(MonitorConfig));
            using var conext = new ApplicationContext();
            conext.MonitorConfigs.Add(monitorConfig);

            conext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Helper.Logger.Error(ex, "Error adding {config}.", nameof(MonitorConfig));
            return false;
        }
    }

    /// <summary>
    /// Loads the current config into memory.
    /// </summary>
    public static bool LoadSavedConfigToMemory()
    {
        try
        {
            Helper.Logger.Verbose("Loading {config} from database into memory.", nameof(MonitorConfig));
            _monitorConfig.Clear();

            // Since we work with a local db and datasets are expected to be very small, everthying is preloaded.
            using var conext = new ApplicationContext();
            var monitorConfigs = conext.MonitorConfigs.Include(i => i.TriggerActions).ToArray();

            foreach (var monitorConfig in conext.MonitorConfigs)
            {
                if (!_monitorConfig.TryAdd(monitorConfig.Id, monitorConfig))
                {
                    Helper.Logger.Warning("Could not add {config} with {key} to memory. Pelase check for duplciattes.",
                        nameof(MonitorConfig), monitorConfig.Id);
                    return false;
                }
            }

            Helper.Logger.Verbose("Sucessfully loaded into {config} from database into memory.", nameof(MonitorConfig));
            return true;
        }
        catch (Exception ex)
        {
            Helper.Logger.Error(ex, "Error loading config into memory.");
            return false;
        }
    }
}
