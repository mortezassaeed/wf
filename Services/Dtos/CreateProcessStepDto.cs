namespace Services.Dtos;

public class CreateProcessStepDto
{
    public int ProcessId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int Order { get; set; }
    public bool IsStart { get; set; }
    public bool IsEnd { get; set; }
    public bool RequiresApproval { get; set; }
    public string? ApproverRoles { get; set; }
    public bool IsActive { get; set; } = true;
    public List<ProcessStepActionDto> Actions { get; set; } = new();
}
