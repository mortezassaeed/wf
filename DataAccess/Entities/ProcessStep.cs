using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class ProcessStep
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProcessId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Code { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    public int Order { get; set; }

    public bool IsStart { get; set; } = false;

    public bool IsEnd { get; set; } = false;

    public bool RequiresApproval { get; set; } = false;

    [MaxLength(500)]
    public string ApproverRoles { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    [ForeignKey(nameof(ProcessId))]
    public virtual Process Process { get; set; }
}
