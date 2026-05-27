using DataAccess.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class ProcessInstanceStep
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProcessInstanceId { get; set; }

    [Required]
    public int ProcessStepId { get; set; }

    public ProcessInstanceStepState State { get; set; } = ProcessInstanceStepState.Pending;

    public int? AssignedToUserId { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    [MaxLength(2000)]
    public string Comments { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    [ForeignKey(nameof(ProcessInstanceId))]
    public virtual ProcessInstance ProcessInstance { get; set; }

    [ForeignKey(nameof(ProcessStepId))]
    public virtual ProcessStep ProcessStep { get; set; }
}
