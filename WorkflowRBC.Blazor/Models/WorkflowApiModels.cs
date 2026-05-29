namespace WorkflowRBC.Blazor.Models;

public static class WorkflowDataTypes
{
    public const string LeaveRequest = "LEAVE_REQUEST";
    public const string PurchaseRequest = "PURCHASE_REQUEST";
}

public enum WorkflowAction
{
    Submit = 1,
    Approve = 2,
    Reject = 3,
    Cancel = 4,
    RequestMoreInfo = 5,
    ProvideInfo = 6,
    Revise = 7,
    Complete = 8
}

public enum ProcessInstanceState
{
    Open = 1,
    InProgress = 2,
    Completed = 3,
    Rejected = 4,
    Cancelled = 5,
    OnHold = 6
}

public enum ProcessInstanceStepState
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Skipped = 4,
    Active = 5
}

public sealed class ProcessDto
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public List<string> AllowedDataTypes { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ProcessStepDto> Steps { get; set; } = new();
    public List<ProcessStepActionDto> StepActions { get; set; } = new();
}

public sealed class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}

public sealed class ProcessStepDto
{
    public int Id { get; set; }
    public int ProcessId { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int Order { get; set; }
    public bool IsStart { get; set; }
    public bool IsEnd { get; set; }
    public bool RequiresApproval { get; set; }
    public bool CanEditData { get; set; } = true;
    public string? ApproverRoles { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class ProcessStepActionDto
{
    public int Id { get; set; }
    public WorkflowAction Action { get; set; }
    public string DisplayName { get; set; } = "";
    public bool IsActive { get; set; }
    public int FromStepId { get; set; }
    public int ToStepId { get; set; }
}

public sealed class CreateProcessDto
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> AllowedDataTypes { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public List<ProcessStepDto> Steps { get; set; } = new();
}

public sealed class ProcessDataTypeDto
{
    public string Code { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string EntityType { get; set; } = "";
    public string DtoType { get; set; } = "";
    public List<ProcessDataFieldDto> Fields { get; set; } = new();
}

public sealed class ProcessDataFieldDto
{
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Type { get; set; } = "";
    public bool Required { get; set; }
    public int? MaxLength { get; set; }
    public double? Minimum { get; set; }
    public double? Maximum { get; set; }
    public string? HelpText { get; set; }
    public string? Placeholder { get; set; }
    public string? ControlType { get; set; }
    public int Order { get; set; }
    public List<string> Options { get; set; } = new();
}

public sealed class CreateProcessStepActionDto
{
    public int ProcessId { get; set; }
    public int FromStepId { get; set; }
    public int ToStepId { get; set; }
    public WorkflowAction Action { get; set; }
    public string DisplayName { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

public sealed class CreateProcessInstanceRequest
{
    public string ProcessCode { get; set; } = "";
    public string Title { get; set; } = "";
    public string DataType { get; set; } = "";
    public object Data { get; set; } = new();
}

public sealed class ProcessInstanceDataRequest
{
    public string DataType { get; set; } = "";
    public object Data { get; set; } = new();
    public int UserId { get; set; } = 1;
}

public sealed class ExecuteActionRequest
{
    public int ProcessInstanceId { get; set; }
    public WorkflowAction Action { get; set; }
    public int PerformedByUserId { get; set; }
    public string Comments { get; set; } = "";
}

public sealed class CartableItemDto
{
    public int ProcessInstanceId { get; set; }
    public string ProcessCode { get; set; } = "";
    public string ProcessName { get; set; } = "";
    public string Title { get; set; } = "";
    public ProcessInstanceState InstanceState { get; set; }
    public int ProcessInstanceStepId { get; set; }
    public int ProcessStepId { get; set; }
    public string StepCode { get; set; } = "";
    public string StepName { get; set; } = "";
    public bool CanEditData { get; set; } = true;
    public ProcessInstanceStepState StepState { get; set; }
    public int? AssignedToUserId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class ProcessInstanceDto
{
    public int Id { get; set; }
    public int ProcessId { get; set; }
    public string? ProcessCode { get; set; }
    public string? ProcessName { get; set; }
    public int CreatedByUserId { get; set; }
    public string Title { get; set; } = "";
    public ProcessInstanceState State { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ProcessInstanceStepDto> Steps { get; set; } = new();
    public List<ProcessInstanceHistoryDto> History { get; set; } = new();
    public object? Data { get; set; }
}

public sealed class ProcessInstanceStepDto
{
    public int Id { get; set; }
    public int ProcessInstanceId { get; set; }
    public int ProcessStepId { get; set; }
    public ProcessInstanceStepState State { get; set; }
    public int? AssignedToUserId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Comments { get; set; }
}

public sealed class ProcessInstanceHistoryDto
{
    public int Id { get; set; }
    public int ProcessInstanceId { get; set; }
    public int ProcessStepId { get; set; }
    public WorkflowAction Action { get; set; }
    public int PerformedByUserId { get; set; }
    public string? Comments { get; set; }
    public DateTime PerformedAt { get; set; }
    public string? FromState { get; set; }
    public string? ToState { get; set; }
}

public sealed class LeaveRequestDataDto
{
    public int Id { get; set; }
    public int ProcessInstanceId { get; set; }
    public string DataType { get; set; } = WorkflowDataTypes.LeaveRequest;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string LeaveType { get; set; } = "";
    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(7);
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(9);
    public int TotalDays { get; set; } = 3;
    public string Reason { get; set; } = "";
    public string ContactDuringLeave { get; set; } = "";
    public bool IsEmergency { get; set; }
    public int? BackupPersonId { get; set; }
}

public sealed class PurchaseRequestDataDto
{
    public int Id { get; set; }
    public int ProcessInstanceId { get; set; }
    public string DataType { get; set; } = WorkflowDataTypes.PurchaseRequest;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string ItemDescription { get; set; } = "";
    public string Category { get; set; } = "";
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public string VendorName { get; set; } = "";
    public string VendorContact { get; set; } = "";
    public string Justification { get; set; } = "";
    public string BudgetCode { get; set; } = "";
    public DateTime? RequiredByDate { get; set; } = DateTime.Today.AddDays(14);
    public bool IsUrgent { get; set; }
}

public sealed class ApiResult<T>
{
    public bool Succeeded { get; init; }
    public T? Value { get; init; }
    public string Message { get; init; } = "";

    public static ApiResult<T> Success(T value) => new() { Succeeded = true, Value = value };
    public static ApiResult<T> Failure(string message) => new() { Succeeded = false, Message = message };
}
