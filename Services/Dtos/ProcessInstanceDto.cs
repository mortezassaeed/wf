using DataAccess.Enums;

namespace Services.Dtos;

public class ProcessInstanceDto : BaseDto
{
    public int ProcessId { get; set; }
    public string ProcessCode { get; set; }
    public string ProcessName { get; set; }
    public int CreatedByUserId { get; set; }
    public string Title { get; set; }
    public ProcessInstanceState State { get; set; }
    public string StateDisplay { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ProcessInstanceStepDto CurrentStep { get; set; }
    public List<ProcessInstanceStepDto> Steps { get; set; } = new();
    public List<ProcessInstanceHistoryDto> History { get; set; } = new();
    public dynamic? Data { get; set; }
}
