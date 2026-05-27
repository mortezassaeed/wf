namespace Services.Dtos;

public class WorkflowDefinitionDto
{
    public string ProcessCode { get; set; }
    public string ProcessName { get; set; }
    public List<WorkflowStepDefinitionDto> Steps { get; set; } = new();
    public List<WorkflowTransitionDefinitionDto> Transitions { get; set; } = new();
}
