using Buttplug.Client;
using ProjectMerlin.Core.Business;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMerlin.Core.Models;

/// <summary>
/// The action that occurs when a <see cref="MonitorConfig"/> reports a positive.
/// </summary>
[Table(nameof(TriggerAction))]
public sealed class TriggerAction(ButtplugClientDevice? buttplugClientDevice = null)
{
    /// <summary>
    /// The PK/UI/ID...
    /// </summary>
    [Key]
    public Guid Id { get; init; }

    /// <summary>
    /// The human-readable name, displayed to the user.
    /// </summary>
    [MaxLength(100)]
    public string Name { get; init; } = buttplugClientDevice?.Name ?? string.Empty;

    public double Intensity { get; init; }

    public int Duration { get; init; }

    [NotMapped]
    public ButtplugClientDevice? Device { get; set; } = buttplugClientDevice;
    public async Task ExecuteAsync(ButtplugManager buttplugManager)
    {
        if (Device == null)
        {
            Helper.Logger.Verbose("{model} missing {device}.", nameof(TriggerAction), nameof(ButtplugClientDevice));

            if (buttplugManager.GetMatchingDevice(this) is not { } foundDevice)
            {
                Helper.Logger.Warning("Could not find {device} with name '{name}' for {model}", Name, nameof(ButtplugClientDevice), nameof(TriggerAction));
                return;
            }
            Device = foundDevice;
        }

        await Device.VibrateAsync(Enumerable.Repeat(Intensity, 4));
        await Task.Delay(Duration);
        await Device.VibrateAsync(Enumerable.Repeat(0d, 4));
    }
}
