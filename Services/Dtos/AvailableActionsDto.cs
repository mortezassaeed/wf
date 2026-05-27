namespace Services.Dtos;

public class AvailableActionsDto
{
    public int ProcessInstanceId { get; set; }
    public int CurrentStepId { get; set; }
    public string CurrentStepName { get; set; }
    public List<ActionOptionDto> Actions { get; set; } = new();
}
