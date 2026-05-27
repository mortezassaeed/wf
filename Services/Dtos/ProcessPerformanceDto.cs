namespace Services.Dtos;

public class ProcessPerformanceDto
{
    public string ProcessCode { get; set; }
    public string ProcessName { get; set; }
    public int TotalInstances { get; set; }
    public double AverageCompletionTimeHours { get; set; }
    public int CompletedCount { get; set; }
    public int PendingCount { get; set; }
}
