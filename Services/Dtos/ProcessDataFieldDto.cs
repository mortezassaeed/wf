namespace Services.Dtos;

public class ProcessDataFieldDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool Required { get; set; }
    public int? MaxLength { get; set; }
    public double? Minimum { get; set; }
    public double? Maximum { get; set; }
    public string? HelpText { get; set; }
    public string? Placeholder { get; set; }
    public string? ControlType { get; set; }
    public int Order { get; set; }
    public List<string> Options { get; set; } = new();
}
