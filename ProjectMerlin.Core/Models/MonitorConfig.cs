using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Threading.Tasks.Dataflow;

namespace ProjectMerlin.Core.Models;

/// <summary>
/// Contains the data required to setup a monitor.
/// </summary>
[Table(nameof(MonitorConfig))]
public sealed class MonitorConfig
{
    /// <summary>
    /// The PK/UI/ID...
    /// </summary>
    [Key]
    public Guid Id { get; init; }

    /// <summary> The human-readable name, displayed to the user.</summary>
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;
    public bool IsEnabled { get; init; }

    public int PosX { get; init; }
    public int PosY { get; init; }
    public int ArgbInt { get; init; }
    public double Threhshold { get; init; }

    [NotMapped]
    public Color Color { get => field == default ? Color.FromArgb(ArgbInt) : field; set; }


    /// <summary> Navigation property, by convention.</summary>
    public List<TriggerAction> TriggerActions { get; init; } = [];
}
