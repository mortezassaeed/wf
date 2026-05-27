namespace Services.Dtos;

public class CreateProcessInstanceDto
{
    public int ProcessId { get; set; }
    public int CreatedByUserId { get; set; }
    public string Title { get; set; }
    public IProcessDataDto Data { get; set; }
}
