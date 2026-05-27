namespace Services.Dtos;

public class RecruitmentRequestDto : ProcessInstanceDataBaseDto
{
    public override string DataType => "RECRUITMENT_REQUEST";

    public string PositionTitle { get; set; }
    public string Department { get; set; }
    public string EmploymentType { get; set; }
    public int NumberOfPositions { get; set; } = 1;
    public string SeniorityLevel { get; set; }
    public string JobDescription { get; set; }
    public string RequiredQualifications { get; set; }
    public string PreferredQualifications { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public DateTime? TargetStartDate { get; set; }
    public string ReasonForHiring { get; set; }
    public bool IsReplacement { get; set; }
    public int? ReplacingEmployeeId { get; set; }
}
