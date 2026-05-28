namespace Services.Dtos;

public class CreateProcessInstanceDto
{
    public string ProcessCode { get; set; }
    public string Title { get; set; }
    public IProcessDataDto? Data { get; set; }
}
