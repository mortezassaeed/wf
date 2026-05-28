using DataAccess.Enums;

namespace Services.Dtos;

public class CreateProcessStepActionDto
{
    public int ProcessId { get; set; }
    public int FromStepId { get; set; }
    public int ToStepId { get; set; }
    public WorkflowAction Action { get; set; }
    public string DisplayName { get; set; }
    public bool IsActive { get; set; } = true;
}
