namespace Services.Dtos;

public class WorkflowStepDefinitionDto
{
    public string Code { get; set; }
    public string Name { get; set; }
    public bool IsStart { get; set; }
    public bool IsEnd { get; set; }
    public bool RequiresApproval { get; set; }
    public bool CanEditData { get; set; } = true;
    public List<string> ApproverRoles { get; set; } = new();
    public List<string> AvailableActions { get; set; } = new();
}
