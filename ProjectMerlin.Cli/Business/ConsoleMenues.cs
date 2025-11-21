using ProjectMerlin.Cli.Enums;
using ProjectMerlin.Cli.Linux;
using ProjectMerlin.Core.Business;
using ProjectMerlin.Core.Models;

namespace ProjectMerlin.Cli.Business;

public static class ConsoleMenues
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
                    var monitorConfig = Helper.PopulateInstanceFromConsole<MonitorConfig>();
                    if (monitorConfig == null)
                        return 3;
                    MonitorManager.AddMonitorConfig(monitorConfig);
                    break;

                case MainMenuOptions.Run:
                    var monitorManager = new MonitorManager();
                    await monitorManager.InitializeAsync();
                    await monitorManager.Run(new LinuxGrep());
                    break;

                case MainMenuOptions.ResetDatabase:

                    break;

                default:
                    Console.WriteLine("Option not implemented. Please contact the developer.");
                    return 2;
            }
        }
    }
}
