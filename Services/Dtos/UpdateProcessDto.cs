namespace Services.Dtos;

public class UpdateProcessDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> AllowedDataTypes { get; set; } = new();
    public bool IsActive { get; set; }
}
