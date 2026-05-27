using DataAccess.Enums;

namespace Services.Dtos;

public class CreateProcessStepActionDto
{
    public int ProcessStepId { get; set; }
    public WorkflowAction Action { get; set; }
    public string DisplayName { get; set; }
    public bool IsActive { get; set; } = true;
}
