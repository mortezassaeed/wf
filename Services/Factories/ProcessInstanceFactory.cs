using DataAccess.Entities;
using DataAccess.Enums;

namespace Services.Factories;

public static class ProcessInstanceFactory
{
    public static ProcessInstance CreateInstance(
        Process process,
        ProcessStep startStep,
        ProcessInstanceDataBase data,
        string title,
        int userId)
    {
        return new ProcessInstance
        {
            ProcessId = process.Id,
            CreatedByUserId = userId,
            Title = title,
            State = ProcessInstanceState.Open,
            CreatedAt = DateTime.UtcNow,
            Data = data,
            ProcessInstanceSteps = new List<ProcessInstanceStep>
            {
                CreateActiveStep(startStep.Id, userId)
            }
        };
    }

    public static ProcessInstanceHistory CreateHistory(
        int processInstanceId,
        ProcessInstanceStep currentStep,
        WorkflowAction action,
        int performedByUserId,
        string comments,
        ProcessInstanceState fromState,
        ProcessInstanceState toState)
    {
        return new ProcessInstanceHistory
        {
            ProcessInstanceId = processInstanceId,
            ProcessStepId = currentStep.ProcessStepId,
            Action = action,
            PerformedByUserId = performedByUserId,
            Comments = comments,
            PerformedAt = DateTime.UtcNow,
            FromState = fromState.ToString(),
            ToState = toState.ToString()
        };
    }

    public static ProcessInstanceStep CreateActiveStep(int processInstanceId, int processStepId, int assignedToUserId)
    {
        var step = CreateActiveStep(processStepId, assignedToUserId);
        step.ProcessInstanceId = processInstanceId;
        return step;
    }

    private static ProcessInstanceStep CreateActiveStep(int processStepId, int assignedToUserId)
    {
        return new ProcessInstanceStep
        {
            ProcessStepId = processStepId,
            State = ProcessInstanceStepState.Active,
            AssignedToUserId = assignedToUserId,
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Comments = string.Empty
        };
    }
}
