namespace Services.Dtos;

public class LeaveRequestDto : ProcessInstanceDataBaseDto
{
    public override string DataType => "LEAVE_REQUEST";

    [ProcessDataField(Label = "Leave type", Placeholder = "Annual, sick, unpaid", Order = 10)]
    public string LeaveType { get; set; }

    [ProcessDataField(Label = "Start date", ControlType = "date", Order = 20)]
    public DateTime StartDate { get; set; }

    [ProcessDataField(Label = "End date", ControlType = "date", Order = 30)]
    public DateTime EndDate { get; set; }

    [ProcessDataField(Label = "Total days", Order = 40)]
    public int TotalDays { get; set; }

    [ProcessDataField(Label = "Reason", ControlType = "textarea", Order = 50)]
    public string Reason { get; set; }

    [ProcessDataField(Label = "Contact during leave", Order = 60)]
    public string ContactDuringLeave { get; set; }

    [ProcessDataField(Label = "Emergency leave", Order = 70)]
    public bool IsEmergency { get; set; }

    [ProcessDataField(Label = "Backup person id", HelpText = "Optional user id of the backup person.", Order = 80)]
    public int? BackupPersonId { get; set; }
}
