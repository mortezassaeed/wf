using DataAccess.Enums;

namespace Services.Dtos;

public class CartableItemDto
{
    public int ProcessInstanceId { get; set; }
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public ProcessInstanceState InstanceState { get; set; }
    public int ProcessInstanceStepId { get; set; }
    public int ProcessStepId { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public ProcessInstanceStepState StepState { get; set; }
    public int? AssignedToUserId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
