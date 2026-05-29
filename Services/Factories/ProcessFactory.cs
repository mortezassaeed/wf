using DataAccess.Entities;
using Services.Dtos;

namespace Services.Factories;

public static class ProcessFactory
{
    public static Process CreateProcess(CreateProcessDto dto, IEnumerable<string> allowedDataTypes)
    {
        return new Process
        {
            Code = dto.Code.Trim(),
            Name = dto.Name.Trim(),
            Description = dto.Description ?? string.Empty,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            ProcessAllowedDataTypes = CreateAllowedDataTypes(allowedDataTypes),
            ProcessSteps = dto.Steps.Select(CreateStep).ToList()
        };
    }

    public static ProcessStep CreateStep(int processId, CreateProcessStepDto dto)
    {
        var step = CreateStep(dto);
        step.ProcessId = processId;
        return step;
    }

    public static ProcessStepAction CreateAction(int processId, CreateProcessStepActionDto dto)
    {
        return new ProcessStepAction
        {
            ProcessId = processId,
            FromStepId = dto.FromStepId,
            ToStepId = dto.ToStepId,
            Action = dto.Action,
            DisplayName = dto.DisplayName ?? string.Empty,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static List<ProcessAllowedDataType> CreateAllowedDataTypes(IEnumerable<string> dataTypes)
    {
        return dataTypes
            .Select(dataType => new ProcessAllowedDataType { DataType = dataType })
            .ToList();
    }

    private static ProcessStep CreateStep(ProcessStepDto dto)
    {
        return new ProcessStep
        {
            Code = dto.Code ?? string.Empty,
            Name = dto.Name ?? string.Empty,
            Description = dto.Description ?? string.Empty,
            Order = dto.Order,
            IsStart = dto.IsStart,
            IsEnd = dto.IsEnd,
            RequiresApproval = dto.RequiresApproval,
            CanEditData = dto.CanEditData,
            ApproverRoles = dto.ApproverRoles ?? string.Empty,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static ProcessStep CreateStep(CreateProcessStepDto dto)
    {
        return new ProcessStep
        {
            Code = dto.Code ?? string.Empty,
            Name = dto.Name ?? string.Empty,
            Description = dto.Description ?? string.Empty,
            Order = dto.Order,
            IsStart = dto.IsStart,
            IsEnd = dto.IsEnd,
            RequiresApproval = dto.RequiresApproval,
            CanEditData = dto.CanEditData,
            ApproverRoles = dto.ApproverRoles ?? string.Empty,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };
    }
}
