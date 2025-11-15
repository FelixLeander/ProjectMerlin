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
        var texts = new string[] { "EnterValue1", "EnterValue2", "EnterValue3" };
        var list = new List<int>();
        foreach (var text in texts)
        {
            var result = GetInt(text);
            if (result == null)
                break;
            list.Add(result);
        }

        return null;
    }

    private static int? GetInt(string text)
    {
        Console.WriteLine(text);
        var rawInput = Console.ReadLine();
        return int.TryParse(rawInput, out var result) ? result : null;
    }
}
