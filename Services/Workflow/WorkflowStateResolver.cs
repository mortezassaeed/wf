using DataAccess.Enums;

namespace Services.Workflow;

public static class WorkflowStateResolver
{
    public static ProcessInstanceState DetermineNewState(
        WorkflowAction action,
        ProcessInstanceState currentState)
    {
        return action switch
        {
            WorkflowAction.Submit => ProcessInstanceState.InProgress,
            WorkflowAction.Approve => ProcessInstanceState.InProgress,
            WorkflowAction.Complete => ProcessInstanceState.Completed,
            WorkflowAction.Reject => ProcessInstanceState.Rejected,
            WorkflowAction.Cancel => ProcessInstanceState.Cancelled,
            WorkflowAction.RequestMoreInfo => ProcessInstanceState.OnHold,
            WorkflowAction.ProvideInfo => ProcessInstanceState.InProgress,
            _ => currentState
        };
    }
}
