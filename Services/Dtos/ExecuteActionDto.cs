using DataAccess.Enums;

namespace Services.Dtos;

public class ExecuteActionDto
{
    public int ProcessInstanceId { get; set; }
    public WorkflowAction Action { get; set; }
    public int PerformedByUserId { get; set; }
    public string Comments { get; set; } = string.Empty;
    public int? AssignToUserId { get; set; }
}
