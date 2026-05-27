// WorkflowEngine.WebAPI/Controllers/ProcessController.cs
using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Dtos;

namespace WorkflowEngine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessInstancesController : ControllerBase
{
    private readonly IProcessInstanceRepository _instanceRepository;
    private readonly IProcessRepository _processRepository;
    private readonly IRepository<ProcessInstanceStep> _instanceStepRepository;

    public ProcessInstancesController(
        IProcessInstanceRepository instanceRepository,
        IProcessRepository processRepository,
        IRepository<ProcessInstanceStep> instanceStepRepository)
    {
        _instanceRepository = instanceRepository;
        _processRepository = processRepository;
        _instanceStepRepository = instanceStepRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProcessInstanceSummaryDto>>> GetAll([FromQuery] ProcessInstanceFilterDto filter)
    {
        IEnumerable<ProcessInstance> instances;

        //if (filter.UserId.HasValue)
        //{
        //    instances = await _instanceRepository.GetByUserAsync(filter.UserId.Value);
        //}
        if (filter.State.HasValue)
        {
            instances = await _instanceRepository.GetByStateAsync(filter.State.Value);
        }
        else
        {
            instances = await _instanceRepository.GetAllAsync();
        }

        if (!string.IsNullOrEmpty(filter.ProcessCode))
        {
            instances = instances.Where(i => i.Process?.Code == filter.ProcessCode);
        }

        if (filter.CreatedFrom.HasValue)
        {
            instances = instances.Where(i => i.CreatedAt >= filter.CreatedFrom.Value);
        }

        if (filter.CreatedTo.HasValue)
        {
            instances = instances.Where(i => i.CreatedAt <= filter.CreatedTo.Value);
        }

        var dtos = instances.Select(MapToSummaryDto);
        return Ok(dtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProcessInstanceDto>> GetById(int id)
    {
        var instance = await _instanceRepository.GetWithStepsAsync(id);
        if (instance == null)
            return NotFound(new { Message = $"Process instance with ID {id} not found" });

        return Ok(MapToDetailDto(instance));
    }

    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<IEnumerable<ProcessInstanceHistoryDto>>> GetHistory(int id)
    {
        var instance = await _instanceRepository.GetWithHistoryAsync(id);
        if (instance == null)
            return NotFound(new { Message = $"Process instance with ID {id} not found" });

        var dtos = instance.ProcessInstanceHistories?.Select(MapHistoryToDto) ?? Enumerable.Empty<ProcessInstanceHistoryDto>();
        return Ok(dtos);
    }

    [HttpGet("{id:int}/current-step")]
    public async Task<ActionResult<ProcessInstanceStepDto>> GetCurrentStep(int id)
    {
        var currentStep = await _instanceRepository.GetCurrentStepAsync(id);
        if (currentStep == null)
            return NotFound(new { Message = $"No current step found for instance {id}" });

        return Ok(MapStepToDto(currentStep));
    }

    [HttpGet("user/{userId:int}/pending")]
    public async Task<ActionResult<IEnumerable<ProcessInstanceStepDto>>> GetPendingSteps(int userId)
    {
        var steps = await _instanceRepository.GetPendingStepsByUserAsync(userId);
        var dtos = steps.Select(MapStepToDto);
        return Ok(dtos);
    }

    [HttpPost]
    public async Task<ActionResult<ProcessInstanceDto>> Create([FromBody] CreateProcessInstanceDto dto)
    {
        var process = await _processRepository.GetByIdAsync(dto.ProcessId);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {dto.ProcessId} not found" });

        if (!process.IsActive)
            return BadRequest(new { Message = "Cannot create instance for inactive process" });

        var startStep = process.ProcessSteps?.FirstOrDefault(s => s.IsStart);
        if (startStep == null)
            return BadRequest(new { Message = "Process has no start step defined" });

        var instance = new ProcessInstance
        {
            ProcessId = dto.ProcessId,
            CreatedByUserId = dto.CreatedByUserId,
            Title = dto.Title,
            State = ProcessInstanceState.Open,
            CreatedAt = DateTime.UtcNow,
            ProcessInstanceSteps = new List<ProcessInstanceStep>
            {
                new ProcessInstanceStep
                {
                    ProcessStepId = startStep.Id,
                    State = ProcessInstanceStepState.Active,
                    AssignedToUserId = dto.CreatedByUserId,
                    StartedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                }
            }
        };

        await _instanceRepository.AddAsync(instance);
        return CreatedAtAction(nameof(GetById), new { id = instance.Id }, MapToDetailDto(instance));
    }

    [HttpPost("{id:int}/execute")]
    public async Task<ActionResult<ProcessInstanceDto>> ExecuteAction(int id, [FromBody] ExecuteActionDto dto)
    {
        var instance = await _instanceRepository.GetWithStepsAsync(id);
        if (instance == null)
            return NotFound(new { Message = $"Process instance with ID {id} not found" });

        var currentStep = instance.ProcessInstanceSteps?
            .FirstOrDefault(s => s.State == ProcessInstanceStepState.Active);

        if (currentStep == null)
            return BadRequest(new { Message = "No active step found" });

        // Record history
        var history = new ProcessInstanceHistory
        {
            ProcessInstanceId = id,
            ProcessStepId = currentStep.ProcessStepId,
            Action = dto.Action,
            PerformedByUserId = dto.PerformedByUserId,
            Comments = dto.Comments,
            PerformedAt = DateTime.UtcNow,
            //FromState = instance.State,
            //ToState = DetermineNewState(dto.Action, instance.State)
        };

        // Update current step
        currentStep.State = ProcessInstanceStepState.Completed;
        currentStep.CompletedAt = DateTime.UtcNow;
        currentStep.Comments = dto.Comments;
        currentStep.UpdatedAt = DateTime.UtcNow;

        // Update instance state
        //instance.State = history.ToState;
        instance.UpdatedAt = DateTime.UtcNow;

        await _instanceStepRepository.UpdateAsync(currentStep);
        await _instanceRepository.UpdateAsync(instance);

        return Ok(MapToDetailDto(instance));
    }

    [HttpPut("{id:int}/state")]
    public async Task<ActionResult<ProcessInstanceDto>> UpdateState(int id, [FromBody] ProcessInstanceState newState)
    {
        var instance = await _instanceRepository.GetByIdAsync(id);
        if (instance == null)
            return NotFound(new { Message = $"Process instance with ID {id} not found" });

        instance.State = newState;
        instance.UpdatedAt = DateTime.UtcNow;

        await _instanceRepository.UpdateAsync(instance);
        return Ok(MapToSummaryDto(instance));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var instance = await _instanceRepository.GetByIdAsync(id);
        if (instance == null)
            return NotFound(new { Message = $"Process instance with ID {id} not found" });

        await _instanceRepository.DeleteAsync(instance.Id);
        return NoContent();
    }

    private static ProcessInstanceState DetermineNewState(WorkflowAction action, ProcessInstanceState currentState)
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

    private static ProcessInstanceSummaryDto MapToSummaryDto(ProcessInstance instance)
    {
        return new ProcessInstanceSummaryDto
        {
            Id = instance.Id,
            //ProcessId = instance.ProcessId,
            ProcessName = instance.Process?.Name,
            ProcessCode = instance.Process?.Code,
            Title = instance.Title,
            State = instance.State,
            CreatedByUserId = instance.CreatedByUserId,
            CreatedAt = instance.CreatedAt,
            //UpdatedAt = instance.UpdatedAt
        };
    }

    private static ProcessInstanceDto MapToDetailDto(ProcessInstance instance)
    {
        return new ProcessInstanceDto
        {
            Id = instance.Id,
            ProcessId = instance.ProcessId,
            ProcessName = instance.Process?.Name,
            ProcessCode = instance.Process?.Code,
            Title = instance.Title,
            State = instance.State,
            CreatedByUserId = instance.CreatedByUserId,
            CreatedAt = instance.CreatedAt,
            UpdatedAt = instance.UpdatedAt,
            Steps = instance.ProcessInstanceSteps?.Select(MapStepToDto).ToList(),
            History = instance.ProcessInstanceHistories?.Select(MapHistoryToDto).ToList()
        };
    }

    private static ProcessInstanceStepDto MapStepToDto(ProcessInstanceStep step)
    {
        return new ProcessInstanceStepDto
        {
            Id = step.Id,
            ProcessInstanceId = step.ProcessInstanceId,
            ProcessStepId = step.ProcessStepId,
            //StepName = step.ProcessStep?.Name,
            //StepCode = step.ProcessStep?.Code,
            State = step.State,
            AssignedToUserId = step.AssignedToUserId,
            StartedAt = step.StartedAt,
            CompletedAt = step.CompletedAt,
            Comments = step.Comments,
            CreatedAt = step.CreatedAt,
            UpdatedAt = step.UpdatedAt
        };
    }

    private static ProcessInstanceHistoryDto MapHistoryToDto(ProcessInstanceHistory history)
    {
        return new ProcessInstanceHistoryDto
        {
            Id = history.Id,
            ProcessInstanceId = history.ProcessInstanceId,
            ProcessStepId = history.ProcessStepId,
            //StepName = history.ProcessStep?.Name,
            Action = history.Action,
            PerformedByUserId = history.PerformedByUserId,
            Comments = history.Comments,
            PerformedAt = history.PerformedAt,
            FromState = history.FromState,
            ToState = history.ToState
        };
    }
}
