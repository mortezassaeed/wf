namespace Services.Dtos;

public class LeaveRequestDto : ProcessInstanceDataBaseDto
{
    public override string DataType => "LEAVE_REQUEST";

    public string LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public string Reason { get; set; }
    public string ContactDuringLeave { get; set; }
    public bool IsEmergency { get; set; }
    public int? BackupPersonId { get; set; }
}
