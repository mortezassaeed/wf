using System.Text.Json;

namespace Services.Dtos;

public class ProcessInstanceDataRequestDto
{
    public string DataType { get; set; }
    public JsonElement Data { get; set; }
    public int UserId { get; set; } = 1;
}
