namespace Services.Dtos;

public class CreateProcessStepTransitionDto
{
    public int ProcessStepActionId { get; set; }
    public int FromStepId { get; set; }
    public int ToStepId { get; set; }
    public string Condition { get; set; }
    public int Priority { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
