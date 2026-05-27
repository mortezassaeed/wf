namespace Services.Dtos;

public class TaskSummaryDto
{
    public int ProcessInstanceId { get; set; }
    public string ProcessName { get; set; }
    public string Title { get; set; }
    public string CurrentStep { get; set; }
    public DateTime AssignedAt { get; set; }
    public bool IsOverdue { get; set; }

}
