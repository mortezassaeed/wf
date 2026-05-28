namespace Services.Dtos;

public abstract class ProcessInstanceDataBaseDto : IProcessDataDto
{
    public int Id { get; set; }
    public int ProcessInstanceId { get; set; }
    public abstract string DataType { get; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
