using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMerlin.Core.Models;

[Table(nameof(Config))]
public sealed class Config
{
    [Key] 
    public Guid Id { get; init; }
}
