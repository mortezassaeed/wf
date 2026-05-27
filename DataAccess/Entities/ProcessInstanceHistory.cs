using DataAccess.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class ProcessInstanceHistory
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProcessInstanceId { get; set; }

    [Required]
    public int ProcessStepId { get; set; }

    [Required]
    public WorkflowAction Action { get; set; }

    public int PerformedByUserId { get; set; }

    [MaxLength(2000)]
    public string Comments { get; set; }

    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string FromState { get; set; }

    [MaxLength(100)]
    public string ToState { get; set; }

    // Navigation properties
    [ForeignKey(nameof(ProcessInstanceId))]
    public virtual ProcessInstance ProcessInstance { get; set; }

    [ForeignKey(nameof(ProcessStepId))]
    public virtual ProcessStep ProcessStep { get; set; }
}
