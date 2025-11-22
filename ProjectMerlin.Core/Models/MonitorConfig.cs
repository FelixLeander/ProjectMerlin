using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace ProjectMerlin.Core.Models;

/// <summary>
/// Contains the data required to setup a monitor.
/// </summary>
[Table(nameof(MonitorConfig))]
public class MonitorConfig
{
    /// <summary>
    /// The PK/UI/ID...
    /// </summary>
    [Key]
    public Guid Id { get; init; }

    /// <summary> The human-readable name, displayed to the user.</summary>
    [MaxLength(100)]
    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public bool IsEnabled { get; init; } = true;

    [Required]
    public int PosX { get; init; }

    [Required]
    public int PosY { get; init; }

    [Required]
    public int ArgbInt { get; init; }

    public double Threhshold { get; init; } = 0.80;

    [NotMapped]
    public Color Color { get => field == default ? Color.FromArgb(ArgbInt) : field; set; }

    /// <summary> Navigation property, by convention.</summary>
    public List<TriggerAction> TriggerActions { get; init; } = [];
}
