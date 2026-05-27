using DataAccess.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace DataAccess.Entities;

public class ProcessStepAction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public WorkflowAction Action { get; set; }

    [Required]
    [MaxLength(200)]
    public string DisplayName { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }

    public bool IsDeleted { get; set; } = false;

    [Required]
    public int FromStepId { get; set; }

    [Required]
    public int ToStepId { get; set; }
    [Required]
    public int ProcessId { get; set; }


    [ForeignKey(nameof(FromStepId))]
    public virtual ProcessStep FromStep { get; set; }

    [ForeignKey(nameof(ToStepId))]
    public virtual ProcessStep ToStep { get; set; }

    [ForeignKey(nameof(ProcessId))]
    public virtual Process Process { get; set; }

    //public virtual ICollection<ProcessStepTransition> Transitions { get; set; }
}
