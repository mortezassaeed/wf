using DataAccess.Entities;
using Services.Dtos;

namespace Services.Resolver;

public sealed class LeaveRequestDataHandler : ProcessDataHandlerBase<LeaveRequestDto, LeaveRequestData>
{
    public override string DataType => "LEAVE_REQUEST";

    protected override LeaveRequestData CreateEntity(LeaveRequestDto dto)
    {
        return new LeaveRequestData
        {
            LeaveType = dto.LeaveType ?? string.Empty,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TotalDays = dto.TotalDays,
            Reason = dto.Reason ?? string.Empty,
            ContactDuringLeave = dto.ContactDuringLeave ?? string.Empty,
            IsEmergency = dto.IsEmergency,
            BackupPersonId = dto.BackupPersonId
        };
    }

    protected override IProcessDataDto ToDto(LeaveRequestData entity)
    {
        return new LeaveRequestDto
        {
            Id = entity.Id,
            ProcessInstanceId = entity.ProcessInstanceId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            LeaveType = entity.LeaveType ?? string.Empty,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            TotalDays = entity.TotalDays,
            Reason = entity.Reason ?? string.Empty,
            ContactDuringLeave = entity.ContactDuringLeave ?? string.Empty,
            IsEmergency = entity.IsEmergency,
            BackupPersonId = entity.BackupPersonId
        };
    }
}
