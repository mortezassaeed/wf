using DataAccess.Entities;
using Services.Dtos;

namespace Services.Mappers;

public static class ProcessMapper
{
    public static ProcessDto ToDto(Process process)
    {
        return new ProcessDto
        {
            Id = process.Id,
            Code = process.Code,
            Name = process.Name,
            Description = process.Description ?? string.Empty,
            AllowedDataTypes = process.ProcessAllowedDataTypes?.Select(t => t.DataType).ToList() ?? new List<string>(),
            IsActive = process.IsActive,
            CreatedAt = process.CreatedAt,
            UpdatedAt = process.UpdatedAt,
            Steps = process.ProcessSteps?.Select(ToStepDto).ToList() ?? new List<ProcessStepDto>(),
            StepActions = process.ProcessStepActions?.Select(ToActionDto).ToList() ?? new List<ProcessStepActionDto>()
        };
    }

    public static WorkflowDefinitionDto ToWorkflowDefinitionDto(Process process)
    {
        return new WorkflowDefinitionDto
        {
            ProcessCode = process.Code,
            ProcessName = process.Name,
            Steps = process.ProcessSteps?.Select(s => new WorkflowStepDefinitionDto
            {
                IsStart = s.IsStart,
                IsEnd = s.IsEnd,
                RequiresApproval = s.RequiresApproval
            }).ToList() ?? new List<WorkflowStepDefinitionDto>()
        };
    }

    public static ProcessStepDto ToStepDto(ProcessStep step)
    {
        return new ProcessStepDto
        {
            Id = step.Id,
            ProcessId = step.ProcessId,
            Code = step.Code,
            Name = step.Name,
            Description = step.Description ?? string.Empty,
            Order = step.Order,
            IsStart = step.IsStart,
            IsEnd = step.IsEnd,
            RequiresApproval = step.RequiresApproval,
            ApproverRoles = step.ApproverRoles ?? string.Empty,
            IsActive = step.IsActive,
            CreatedAt = step.CreatedAt,
            UpdatedAt = step.UpdatedAt
        };
    }

    public static ProcessStepActionDto ToActionDto(ProcessStepAction action)
    {
        return new ProcessStepActionDto
        {
            Id = action.Id,
            Action = action.Action,
            DisplayName = action.DisplayName,
            IsActive = action.IsActive,
            FromStepId = action.FromStepId,
            ToStepId = action.ToStepId,
            CreatedAt = action.CreatedAt,
            UpdatedAt = action.UpdatedAt
        };
    }
}
