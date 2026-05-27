using DataAccess.Enums;

namespace Services.Dtos;

public class ActionOptionDto
{
    public WorkflowAction Action { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public bool RequiresComment { get; set; }
    public bool RequiresAssignment { get; set; }
    public List<int> PossibleNextSteps { get; set; } = new();
}
