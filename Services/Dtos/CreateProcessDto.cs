namespace Services.Dtos;

public class CreateProcessDto
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> AllowedDataTypes { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public List<ProcessStepDto> Steps { get; set; } = new();
    public List<ProcessStepActionDto> ProcessStepActions { get; set; } = new();
}
