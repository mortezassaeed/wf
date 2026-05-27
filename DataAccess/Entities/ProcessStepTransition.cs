using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

//[NotMapped]
//public class ProcessStepTransition
//{
//    [Key]
//    public int Id { get; set; }

//    [Required]
//    public int ProcessId { get; set; }

//    [Required]
//    public int FromStepId { get; set; }

//    [Required]
//    public int ToStepId { get; set; }

//    [MaxLength(500)]
//    public string Condition { get; set; }

//    public int Priority { get; set; } = 0;

//    public bool IsActive { get; set; } = true;

//    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

//    public DateTime? UpdatedAt { get; set; }

//    [Timestamp]
//    public byte[] RowVersion { get; set; }

//    public bool IsDeleted { get; set; } = false;

//    // Navigation properties
//    [ForeignKey(nameof(ProcessId))]
//    public virtual Process Process { get; set; }

//    [ForeignKey(nameof(FromStepId))]
//    public virtual ProcessStep FromStep { get; set; }

//    [ForeignKey(nameof(ToStepId))]
//    public virtual ProcessStep ToStep { get; set; }
//}
