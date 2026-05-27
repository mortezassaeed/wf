using DataAccess.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class ProcessInstance
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProcessId { get; set; }

    [Required]
    public int CreatedByUserId { get; set; }

    [MaxLength(200)]
    public string Title { get; set; }

    public ProcessInstanceState State { get; set; } = ProcessInstanceState.Open;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    [ForeignKey(nameof(ProcessId))]
    public virtual Process Process { get; set; }

    public virtual ICollection<ProcessInstanceStep> ProcessInstanceSteps { get; set; }
    public virtual ICollection<ProcessInstanceHistory> ProcessInstanceHistories { get; set; }

    // TPT relationship - one-to-one with specific data type
    public virtual ProcessInstanceDataBase Data { get; set; }
}
