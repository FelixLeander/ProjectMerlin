using System.Globalization;
using System.Numerics;
using ProjectMerlin.Cli.Enums;
using ProjectMerlin.Core.Business;
using ProjectMerlin.Core.Models;

namespace ProjectMerlin.Cli.Business;

public static class Interactive
{
    internal static async Task<int> MainMenuAsync()
    {
        var options = Enum.GetValues<MainMenuOptions>();
        while (true)
        {
            Console.WriteLine("Select an option:");
            for (var i = 0; i < options.Length; i++)
            {
                var item = options[i];
                Console.WriteLine($"  {i}: {item}");
            }

            var rawInput = Console.ReadLine();
            if (!int.TryParse(rawInput, out var option))
            {
                Console.WriteLine("Invalid input.");
                continue;
            }

            switch ((MainMenuOptions)option)
            {
                case MainMenuOptions.Exit:
                    Console.WriteLine("Exiting...");
                    return 0;

                case MainMenuOptions.Configure:
                    var monitorConfig = CreateConfig();
                    if (monitorConfig == null)
                        return 3;
                    MonitorManager.AddMonitorConfig(monitorConfig);
                    break;

                case MainMenuOptions.Run:
                    var monitorManager = new MonitorManager();
                    await monitorManager.LoadConfigAsync();
                    break;

                default:
                    Console.WriteLine("Option not implemented. Please contact the developer.");
                    return 2;
            }
        }
    }

    private static MonitorConfig? CreateConfig() 
    {
        var texts = new[] { "Position X", "Position Y", "Target ARGB", "Percentage diff (0.9 == 90%)" };
        var list = new List<int>();
        foreach (var text in texts)
        {
            var result = GeValue(text);
            if (result == null)
                break;
            list.Add(result ?? 0);
        }

        var monitorConfig = new MonitorConfig()
        {
            PosX = list[0],
            PosY = list[1],
            ArgbInt = list[2],
            Threhshold = list[3],
        };

        return monitorConfig;
    }

    private static T? GeValue<T>(string text) where T : struct, INumberBase<T>
    {
        Console.WriteLine(text);
        var rawInput = Console.ReadLine();
        return T.TryParse(rawInput, CultureInfo.InvariantCulture,  out var result) ? result : null;
    }
}
