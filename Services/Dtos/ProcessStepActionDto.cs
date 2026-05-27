using DataAccess.Enums;

namespace Services.Dtos;

public class ProcessStepActionDto : BaseDto
{
    public int ProcessStepId { get; set; }
    public WorkflowAction Action { get; set; }
    public string DisplayName { get; set; }
    public bool IsActive { get; set; }
    public int FromStepId { get; set; }
    public int ToStepId { get; set; }
    //public List<ProcessStepTransitionDto> Transitions { get; set; } = new();
}
