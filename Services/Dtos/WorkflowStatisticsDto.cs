namespace Services.Dtos;

public class WorkflowStatisticsDto
{
    public int TotalInstances { get; set; }
    public int OpenInstances { get; set; }
    public int InProgressInstances { get; set; }
    public int CompletedInstances { get; set; }
    public int RejectedInstances { get; set; }
    public int CancelledInstances { get; set; }
    public Dictionary<string, int> InstancesByProcess { get; set; } = new();
    public Dictionary<string, int> InstancesByState { get; set; } = new();
    public List<ProcessPerformanceDto> ProcessPerformance { get; set; } = new();
}
