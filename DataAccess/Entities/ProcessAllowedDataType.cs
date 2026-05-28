using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class ProcessAllowedDataType
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProcessId { get; set; }

    [Required]
    [MaxLength(100)]
    public string DataType { get; set; } = string.Empty;

    [ForeignKey(nameof(ProcessId))]
    public virtual Process Process { get; set; } = null!;
}
