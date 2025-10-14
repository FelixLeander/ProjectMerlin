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

namespace ProjectMerlin.Core.Business;


public sealed class MonitorManager
{
    /// <summary>
    /// Contains a collection of configurations.
    /// </summary>
    /// <remarks>
    /// Note, that this instance might be overwritten.
    /// So you should'n keeep a reference to the instance.
    /// </remarks>
    public Dictionary<int, MonitorConfig> MonitorConfig { get; private set; } = [];

    /// <summary>
    /// Adds a new <see cref="Models.MonitorConfig"/> to the databaase.
    /// </summary>
    /// <param name="monitorConfig"></param>
    /// <returns>A <see langword="bool"/> indicattign sucess or failure.</returns>
    public bool Add(MonitorConfig monitorConfig)
    {
        try
        {
            Helper.Logger.Verbose("Adding {config}.", nameof(Models.MonitorConfig));
            using var conext = new ApplicationContext();
            conext.MonitorConfigs.Add(monitorConfig);

            conext.SaveChanges();
            return true;
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
    public void LoadSavedConfigFromDatabase()
    {
        try
        {
            Helper.Logger.Verbose("Loading {config} from database into memory.", nameof(Models.MonitorConfig));

            // Since we work with a local db and datasets are expected to be very small, everthying is preloaded.
            using var conext = new ApplicationContext();
            MonitorConfig = conext.MonitorConfigs
                .AsNoTracking()
                .Include(i => i.TriggerActions)
                .Select(s => KeyValuePair.Create(s.ArgbColor, s))
                .ToDictionary();

            Helper.Logger.Verbose("Sucessfully loaded into {config} from database into memory.", nameof(Models.MonitorConfig));
        }
        catch (Exception ex)
        {
            const string error = "Could not load saved config from the database.";
            Helper.Logger.Error(ex, error);
            throw new Exception(error, ex);
        }
    }
}
