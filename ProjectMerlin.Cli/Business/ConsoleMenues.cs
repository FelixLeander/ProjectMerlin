using Buttplug.Client;
using Microsoft.EntityFrameworkCore;
using ProjectMerlin.Cli.Enums;
using ProjectMerlin.Cli.Linux;
using ProjectMerlin.Core.Business;
using ProjectMerlin.Core.Models;

namespace ProjectMerlin.Cli.Business;

internal static class ConsoleMenues
{
    internal static async Task<int> MainMenuAsync()
    {
        WriteWelcomeMessage();
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
                    try
                    {
                        await ConfigurationMenu();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Error configuring.");
                    }
                    break;

                case MainMenuOptions.Run:
                    try
                    {
                        var monitorManager = new MonitorManager();
                        await monitorManager.InitializeAsync();
                        await monitorManager.Run(new LinuxGrep());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;

                case MainMenuOptions.ResetDatabase:
                    Console.WriteLine("Are you sure you want to reset the database? (y/N)");
                    if ("y".Equals(Console.ReadLine(), StringComparison.OrdinalIgnoreCase))
                        DatabaseManager.Initialize(true);
                    break;

                default:
                    Console.WriteLine("Option not implemented. Please contact the developer.");
                    return 2;
            }
        }
    }

    internal static async Task ConfigurationMenu()
    {
        var options = Enum.GetValues<ConfigMenuOptions>();
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

            switch ((ConfigMenuOptions)option)
            {
                case ConfigMenuOptions.Back:
                    Console.WriteLine("Exiting...");
                    return;

                case ConfigMenuOptions.AddNewMonitorConfig:
                    var monitorConfig = Helper.PopulateInstanceFromConsole<MonitorConfig>();

                    Console.WriteLine("[DEBUG] Add testing trigger? (y/N)"); // TODO: This is a placeholer until TriggerActions are editable.
                    if (!"y".Equals(Console.ReadLine(), StringComparison.OrdinalIgnoreCase))
                        break;

                    try
                    {
                        var bpm = new ButtplugManager();
                        await bpm.ConnectToServerAsync();

                        var devices = bpm.GetCurrentDevices()
                            .Select(s => new TriggerAction(s))
                            .ToArray();

                        monitorConfig.TriggerActions.AddRange(devices);
                        MonitorManager.AddMonitorConfig(monitorConfig);
                    }
                    catch (ButtplugClientConnectorException ex)
                    {
                        Console.WriteLine("Connection to intiface failed.");
                        Console.WriteLine("Enusre intiface is running. Get is here: https://intiface.com/central/");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected error: {ex.Message}");
                    }
                    break;

                case ConfigMenuOptions.EditMonitorConfig:
                    Console.WriteLine("work in progress"); // TODO: FINISH this.
                    //EditConfigMenu();
                    break;

                case ConfigMenuOptions.DeleteMonitorConfig:
                    DeleteConfigMenu();
                    break;

                default:
                    Console.WriteLine("Option not implemented. Please contact the developer.");
                    break;
            }
        }
    }

    internal static void EditConfigMenu<T>(T instance)
    {
        while (true)
        {
            var props = typeof(T).GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
            }
        }
    }

    internal static void DeleteConfigMenu()
    {
        while (true)
        {
            using var getContext = new DatabaseManager();
            var configs = getContext.MonitorConfigs.AsNoTracking().ToArray();
            Console.WriteLine($"Select an {nameof(MonitorConfig)} to delete. (Leave empty to return)");
            if (configs.Length == 0)
            {
                Console.WriteLine("There are no configs to delete.");
                return;
            }

            for (int i = 0; i < configs.Length; i++)
            {
                var config = configs[i];
                Console.WriteLine($"  {i}: '{config.Name}' contains {config.TriggerActions.Count} {nameof(MonitorConfig.TriggerActions)}");
            }

            var rawInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(rawInput))
                return;

            if (!int.TryParse(rawInput, out var option))
            {
                Console.WriteLine("Invalid input.");
                continue;
            }

            if (option >= configs.Length)
            {
                Console.WriteLine($"'{option}' Is out of range.");
                continue;
            }

            var toRemove = configs[option];
            using var removeContext = new DatabaseManager();
            removeContext.MonitorConfigs.Remove(toRemove);
            if (removeContext.SaveChanges() == 0)
                Console.WriteLine("No changes have been made.");
            else
                Console.WriteLine($"Removed '{toRemove.Name}' from the database.");
        }
    }

    internal static void WriteWelcomeMessage()
    {
        Console.WriteLine("=====================================");
        Console.WriteLine("      Welcome to Project Merlin      ");
        Console.WriteLine("  Monitor pixel colors and trigger   ");
        Console.WriteLine("         actions based on them.      ");
        Console.WriteLine("=====================================");
    }
}
