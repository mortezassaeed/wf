using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public abstract class ProcessInstanceDataBase
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProcessInstanceId { get; set; }

    [Required]
    [MaxLength(100)]
    public string DataType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public int? UpdatedByUserId { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Navigation
    [ForeignKey(nameof(ProcessInstanceId))]
    public virtual ProcessInstance ProcessInstance { get; set; }
}
