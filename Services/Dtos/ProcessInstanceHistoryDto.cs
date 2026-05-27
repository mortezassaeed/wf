using DataAccess.Enums;

namespace Services.Dtos;

public class ProcessInstanceHistoryDto
{
    public int Id { get; set; }
    public int ProcessInstanceId { get; set; }
    public int ProcessStepId { get; set; }
    public string ProcessStepName { get; set; }
    public WorkflowAction Action { get; set; }
    public string ActionDisplay { get; set; }
    public int PerformedByUserId { get; set; }
    public string Comments { get; set; }
    public DateTime PerformedAt { get; set; }
    public string FromState { get; set; }
    public string ToState { get; set; }
}
