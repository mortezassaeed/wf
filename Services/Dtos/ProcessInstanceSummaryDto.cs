using DataAccess.Enums;

namespace Services.Dtos;

public class ProcessInstanceSummaryDto
{
    public int Id { get; set; }
    public string ProcessCode { get; set; }
    public string ProcessName { get; set; }
    public string Title { get; set; }
    public ProcessInstanceState State { get; set; }
    public string StateDisplay { get; set; }
    public string CurrentStepName { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
