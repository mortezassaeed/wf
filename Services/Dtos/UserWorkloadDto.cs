namespace Services.Dtos;

public class UserWorkloadDto
{
    public int UserId { get; set; }
    public int AssignedTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public List<TaskSummaryDto> RecentTasks { get; set; } = new();
}
