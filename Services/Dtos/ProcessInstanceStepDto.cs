using DataAccess.Enums;

namespace Services.Dtos;

public class ProcessInstanceStepDto : BaseDto
{
    public int ProcessInstanceId { get; set; }
    public int ProcessStepId { get; set; }
    public string ProcessStepCode { get; set; }
    public string ProcessStepName { get; set; }
    public ProcessInstanceStepState State { get; set; }
    public string StateDisplay { get; set; }
    public int? AssignedToUserId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Comments { get; set; }
    public List<WorkflowAction> AvailableActions { get; set; } = new();
}
