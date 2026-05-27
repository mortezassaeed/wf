namespace Services.Dtos;

public class WorkflowTransitionDefinitionDto
{
    public string FromStepCode { get; set; }
    public string ToStepCode { get; set; }
    public string Action { get; set; }
    public string Condition { get; set; }
}
