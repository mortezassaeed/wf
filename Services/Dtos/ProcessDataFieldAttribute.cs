namespace Services.Dtos;

// Optional UI/API metadata for a workflow data DTO property discovered by ProcessDataTypeProvider.
[AttributeUsage(AttributeTargets.Property)]
public sealed class ProcessDataFieldAttribute : Attribute
{
    public string? Label { get; init; }
    public string? HelpText { get; init; }
    public string? Placeholder { get; init; }
    public string? ControlType { get; init; }
    public int Order { get; init; }
    public bool Hidden { get; init; }
}
