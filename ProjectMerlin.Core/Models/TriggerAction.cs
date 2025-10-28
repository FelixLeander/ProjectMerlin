using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMerlin.Core.Models;

/// <summary>
/// The action that occurs when a <see cref="MonitorConfig"/> reports a positive.
/// </summary>
[Table(nameof(TriggerAction))]
public sealed class TriggerAction
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
    public string Name { get; init; } = string.Empty;
    
    //TODO-Felix: Find out how to map devices.
    public int TargetDevice { get; init; }
}
