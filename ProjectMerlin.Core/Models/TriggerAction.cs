using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMerlin.Core.Models;

[Table(nameof(TriggerAction))]
public sealed class TriggerAction
{
    [Key]
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public int TargetDevice {  get; init; }
}
