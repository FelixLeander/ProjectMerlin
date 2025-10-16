using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace ProjectMerlin.Core.Models;

/// <summary>
/// Contains the data required to setup a monitor.
/// </summary>
[Table(nameof(MonitorConfig))]
public sealed class MonitorConfig
{
    /// <summary>The PK/UI/ID...</summary>
    [Key] public Guid Id { get; init; }
    
    /// <summary> The human-readable name, displayed to the user.</summary>
    [MaxLength(100)] public string Name { get; init; } = string.Empty;
    public bool IsEnabled { get; init; }

    public int PosX { get; init; }
    public int PosY { get; init; }
    public required int ArgbColor { get; init; }

    [NotMapped] public Color Color => Color.FromArgb(ArgbColor);

    /// <summary> Navigation property, by convention.</summary>
    public List<TriggerAction> TriggerActions { get; init; } = [];
}
