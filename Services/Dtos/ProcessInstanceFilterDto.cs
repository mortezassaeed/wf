using DataAccess.Enums;

namespace Services.Dtos;

public class ProcessInstanceFilterDto
{
    public string ProcessCode { get; set; }
    public ProcessInstanceState? State { get; set; }
    public int? CreatedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
