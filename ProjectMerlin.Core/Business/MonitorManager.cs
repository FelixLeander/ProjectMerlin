using System.Drawing;
using ProjectMerlin.Core.Abstraction;
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
    public async Task InittalizeAsync()
    {
        try
        {
            Helper.Logger.Verbose("Loading {config} from database into memory.", nameof(MonitorConfig));

            _monitoringConfig = await DatabaseManager.LoadMonitorConfigAsync();
        }
        catch (Exception ex)
        {
            const string error =
                "Could not load saved config from the database. To prevent corruption, internal data has been cleared.";
            Helper.Logger.Error(ex, error);
            throw new Exception(error, ex);
        }
    }

    public async Task Run(IPixelProvider pixelProvider)
    {
        while (true)
        {
            var matches = GetMatching(pixelProvider);
            foreach (var match in matches)
                foreach (var triggerAction in match.TriggerActions)
                    triggerAction.Execute();

            await Task.Delay(500);
        }
    }

    /// <summary>
    /// Adds a new <see cref="MonitorConfig"/> to the database.
    /// </summary>
    /// <param name="monitorConfig"></param>
    /// <returns>A <see langword="bool"/> indicattign success or failure.</returns>
    public static void AddMonitorConfig(params MonitorConfig[] monitorConfig)
    {
        if (monitorConfig.Length == 0)
            throw new ArgumentException("MonitorConfig must have at least one config.", nameof(monitorConfig));

        try
        {
            Helper.Logger.Verbose("Adding {count} '{config}'s.", monitorConfig.Length, nameof(MonitorConfig));
            using var context = new DatabaseManager();
            context.MonitorConfigs.AddRange(monitorConfig);

            var changes = context.SaveChanges();
            Helper.Logger.Verbose("Saved {amount} changes.", changes);
        }
        catch (Exception ex)
        {
            Helper.Logger.Error(ex, "Error adding {config}.", nameof(MonitorConfig));
            throw new Exception($"Could not add new {nameof(monitorConfig)}", ex);
        }
    }

    public IReadOnlyList<MonitorConfig> GetMatching(IPixelProvider pixelProvider)
    {
        return [.. _monitoringConfig.Where(f => {
            //TODO: The pixel provider should not do the filtering.
            var color = pixelProvider.GetPixelColor(f);
            if (color is not { } nonNull)
                return false;
            return ColorSimilarity(f.Color, nonNull) > f.Threhshold;
        })];

        static double ColorSimilarity(Color c1, Color c2)
        {
            var rDiff = c1.R - c2.R;
            var gDiff = c1.G - c2.G;
            var bDiff = c1.B - c2.B;

            var distance = Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
            var maxDistance = Math.Sqrt(255 * 255 * 3);

            // normalize: 1.0 = identical, 0.0 = opposite
            var result = 1.0 - (distance / maxDistance);
            return result;
        }
    }
}
