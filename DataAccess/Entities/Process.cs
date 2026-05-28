using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class Process
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Code { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public virtual ICollection<ProcessStep> ProcessSteps { get; set; } = new List<ProcessStep>();
    public virtual ICollection<ProcessInstance> ProcessInstances { get; set; } = new List<ProcessInstance>();
    public virtual ICollection<ProcessStepAction> ProcessStepActions { get; set; } = new List<ProcessStepAction>();
    public virtual ICollection<ProcessAllowedDataType> ProcessAllowedDataTypes { get; set; } = new List<ProcessAllowedDataType>();


}
