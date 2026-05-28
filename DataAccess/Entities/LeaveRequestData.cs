using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

[ProcessDataType("LEAVE_REQUEST", "Leave request")]
public class LeaveRequestData : ProcessInstanceDataBase
{
    [Required]
    [MaxLength(50)]
    public string LeaveType { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public int TotalDays { get; set; }

    [MaxLength(2000)]
    public string Reason { get; set; }

    [MaxLength(500)]
    public string ContactDuringLeave { get; set; }

    public bool IsEmergency { get; set; }

    public int? BackupPersonId { get; set; }
}
