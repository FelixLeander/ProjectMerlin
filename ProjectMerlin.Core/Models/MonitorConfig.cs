using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMerlin.Core.Models;

[Table(nameof(MonitorConfig))]
public sealed class MonitorConfig
{
    [Key] 
    public Guid Id { get; init; }
    public bool IsActivated { get; set; }
    public required string Color { get; init; }
    public required string ColoRange { get; init; }
    public int PosX { get; init; }
    public int PosY { get; init; }
    public List<TriggerAction> TriggerActions { get; init; } = [];
}
