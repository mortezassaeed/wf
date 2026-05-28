using DataAccess.Entities;
using Services.Dtos;

namespace Services.Mappers;

public static class ProcessInstanceMapper
{
    public static ProcessInstanceSummaryDto ToSummaryDto(ProcessInstance instance)
    {
        return new ProcessInstanceSummaryDto
        {
            Id = instance.Id,
            ProcessName = instance.Process?.Name ?? string.Empty,
            ProcessCode = instance.Process?.Code ?? string.Empty,
            Title = instance.Title ?? string.Empty,
            State = instance.State,
            CreatedByUserId = instance.CreatedByUserId,
            CreatedAt = instance.CreatedAt,
            CompletedAt = instance.CompletedAt,
            CurrentStepName = instance.ProcessInstanceSteps?
                .FirstOrDefault(s => s.State == DataAccess.Enums.ProcessInstanceStepState.Active)?
                .ProcessStep?.Name ?? string.Empty
        };
    }

    public static ProcessInstanceDto ToDetailDto(ProcessInstance instance, IProcessDataDto? data)
    {
        return new ProcessInstanceDto
        {
            Id = instance.Id,
            ProcessId = instance.ProcessId,
            ProcessName = instance.Process?.Name ?? string.Empty,
            ProcessCode = instance.Process?.Code ?? string.Empty,
            Title = instance.Title ?? string.Empty,
            State = instance.State,
            CreatedByUserId = instance.CreatedByUserId,
            CreatedAt = instance.CreatedAt,
            UpdatedAt = instance.UpdatedAt,
            Steps = instance.ProcessInstanceSteps?.Select(ToStepDto).ToList() ?? new List<ProcessInstanceStepDto>(),
            History = instance.ProcessInstanceHistories?.Select(ToHistoryDto).ToList() ?? new List<ProcessInstanceHistoryDto>(),
            Data = data
        };
    }

    public static ProcessInstanceStepDto ToStepDto(ProcessInstanceStep step)
    {
        return new ProcessInstanceStepDto
        {
            Id = step.Id,
            ProcessInstanceId = step.ProcessInstanceId,
            ProcessStepId = step.ProcessStepId,
            State = step.State,
            AssignedToUserId = step.AssignedToUserId,
            StartedAt = step.StartedAt,
            CompletedAt = step.CompletedAt,
            Comments = step.Comments ?? string.Empty,
            CreatedAt = step.CreatedAt,
            UpdatedAt = step.UpdatedAt
        };
    }

    public static ProcessInstanceHistoryDto ToHistoryDto(ProcessInstanceHistory history)
    {
        return new ProcessInstanceHistoryDto
        {
            Id = history.Id,
            ProcessInstanceId = history.ProcessInstanceId,
            ProcessStepId = history.ProcessStepId,
            Action = history.Action,
            PerformedByUserId = history.PerformedByUserId,
            Comments = history.Comments ?? string.Empty,
            PerformedAt = history.PerformedAt,
            FromState = history.FromState,
            ToState = history.ToState
        };
    }
}
