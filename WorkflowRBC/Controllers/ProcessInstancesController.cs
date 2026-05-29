// WorkflowEngine.WebAPI/Controllers/ProcessController.cs
using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Dtos;
using Services.Factories;
using Services.Mappers;
using Services.Resolver;
using Services.Workflow;

namespace WorkflowEngine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessInstancesController : ControllerBase
{
    private static readonly int LoggedInUserId = 1;

    private readonly IProcessInstanceRepository _instanceRepository;
    private readonly IProcessRepository _processRepository;
    private readonly IRepository<ProcessInstanceStep> _instanceStepRepository;
    private readonly IRepository<ProcessInstanceHistory> _historyRepository;
    private readonly IProcessInstanceDataService _dataService;

    public ProcessInstancesController(
        IProcessInstanceRepository instanceRepository,
        IProcessRepository processRepository,
        IRepository<ProcessInstanceStep> instanceStepRepository,
        IRepository<ProcessInstanceHistory> historyRepository,
        IProcessInstanceDataService dataService)
    {
        _instanceRepository = instanceRepository;
        _processRepository = processRepository;
        _instanceStepRepository = instanceStepRepository;
        _historyRepository = historyRepository;
        _dataService = dataService;
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

        var dtos = instances.Select(ProcessInstanceMapper.ToSummaryDto);
        return Ok(dtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProcessInstanceDto>> GetById(int id)
    {
        var instance = await _instanceRepository.GetWithStepsAsync(id);
        if (instance == null)
            return NotFound(new { Message = $"Process instance with ID {id} not found" });

        var instanceData = await _dataService.GetAsync(id);
        var dto = ProcessInstanceMapper.ToDetailDto(instance, instanceData);

        return Ok(dto);
    }

    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<IEnumerable<ProcessInstanceHistoryDto>>> GetHistory(int id)
    {
        var instance = await _instanceRepository.GetWithHistoryAsync(id);
        if (instance == null)
            return NotFound(new { Message = $"Process instance with ID {id} not found" });

        var dtos = instance.ProcessInstanceHistories?.Select(ProcessInstanceMapper.ToHistoryDto) ?? Enumerable.Empty<ProcessInstanceHistoryDto>();
        return Ok(dtos);
    }

    [HttpGet("{id:int}/current-step")]
    public async Task<ActionResult<ProcessInstanceStepDto>> GetCurrentStep(int id)
    {
        var currentStep = await _instanceRepository.GetCurrentStepAsync(id);
        if (currentStep == null)
            return NotFound(new { Message = $"No current step found for instance {id}" });

        return Ok(ProcessInstanceMapper.ToStepDto(currentStep));
    }

    // Deprecated: use GET api/ProcessInstances/cartable?userId={userId} instead.
    // The cartable endpoint uses the same pending-step filter and returns richer
    // data for UI lists.
    //[HttpGet("user/{userId:int}/pending")]
    //public async Task<ActionResult<IEnumerable<ProcessInstanceStepDto>>> GetPendingSteps(int userId)
    //{
    //    var steps = await _instanceRepository.GetPendingStepsByUserAsync(userId);
    //    var dtos = steps.Select(ProcessInstanceMapper.ToStepDto);
    //    return Ok(dtos);
    //}

    [HttpGet("cartable")]
    public async Task<ActionResult<PagedResultDto<CartableItemDto>>> GetCartable(
        [FromQuery] int? userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var currentUserId = userId ?? LoggedInUserId;
        var steps = await _instanceRepository.GetPendingStepsByUserAsync(currentUserId);
        var dtos = steps
            .OrderByDescending(step => step.StartedAt ?? step.CreatedAt)
            .Select(ProcessInstanceMapper.ToCartableItemDto);

        return Ok(PagedResultDto<CartableItemDto>.Create(dtos, pageNumber, pageSize));
    }

    [HttpGet("archive")]
    public async Task<ActionResult<PagedResultDto<ProcessInstanceSummaryDto>>> GetArchive(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var instances = await _instanceRepository.GetByStateAsync(ProcessInstanceState.Completed);
        var dtos = instances
            .OrderByDescending(instance => instance.CompletedAt ?? instance.UpdatedAt ?? instance.CreatedAt)
            .Select(ProcessInstanceMapper.ToSummaryDto);

        return Ok(PagedResultDto<ProcessInstanceSummaryDto>.Create(dtos, pageNumber, pageSize));
    }

    [HttpPost]
    public async Task<ActionResult<ProcessInstanceDto>> Create([FromBody][ModelBinder(typeof(WorkflowRBC.Data.ProcessDataDtoModelBinder))] CreateProcessInstanceDto dto)
    {
        var process = await _processRepository.GetByCodeAsync(dto.ProcessCode);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {dto.ProcessCode} not found" });

        if (!process.IsActive)
            return BadRequest(new { Message = "Cannot create instance for inactive process" });

        if (dto.Data == null)
            return BadRequest(new { Message = "Process instance data is required" });

        var allowedDataTypes = process.ProcessAllowedDataTypes
            .Select(t => t.DataType)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (!allowedDataTypes.Contains(dto.Data.DataType))
            return BadRequest(new { Message = $"Data type '{dto.Data.DataType}' is not allowed for process '{process.Code}'" });

        var startStep = process.ProcessSteps?.FirstOrDefault(s => s.IsStart);
        if (startStep == null)
            return BadRequest(new { Message = "Process has no start step defined" });

        var instanceData = _dataService.CreateEntity(dto.Data, LoggedInUserId);
        if (instanceData == null)
            return BadRequest(new { Message = "Process instance data is required" });

        var instance = ProcessInstanceFactory.CreateInstance(process, startStep, instanceData, dto.Title, LoggedInUserId);

        await _instanceRepository.AddAsync(instance);
        return CreatedAtAction(nameof(GetById), new { id = instance.Id }, ProcessInstanceMapper.ToDetailDto(instance, dto.Data));
    }

    [HttpPost("{id:int}/execute/{workflowAction:int}")]
    public async Task<ActionResult<ProcessInstanceDto>> ExecuteAction(
        int id,
        int workflowAction,
        [FromBody] ExecuteActionDto? dto)
    {
        var comments = dto?.Comments?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(comments))
            return BadRequest(new { Message = "Step comment is required." });

        if (comments.Length > 2000)
            return BadRequest(new { Message = "Step comment cannot be longer than 2000 characters." });

        var instance = await _instanceRepository.GetWithStepsAsync(id);
        if (instance == null)
            return NotFound(new { Message = $"Process instance with ID {id} not found" });

        var process = await _processRepository.GetWithStepsAsync(instance.ProcessId);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {instance.ProcessId} not found" });

        var currentStep = instance.ProcessInstanceSteps?
            .FirstOrDefault(s => s.State == ProcessInstanceStepState.Active);

        if (currentStep == null)
            return BadRequest(new { Message = "No active step found" });

        var stepAction = process.ProcessStepActions?
            .FirstOrDefault(a =>
                a.FromStepId == currentStep.ProcessStepId &&
                a.Action == (WorkflowAction)workflowAction &&
                a.IsActive);

        if (stepAction == null)
            return BadRequest(new { Message = $"Action '{workflowAction}' is not available for the current step" });

        var nextStep = process.ProcessSteps?
            .FirstOrDefault(s => s.Id == stepAction.ToStepId && s.IsActive);

        if (nextStep == null)
            return BadRequest(new { Message = $"Next step with ID {stepAction.ToStepId} was not found or is inactive" });

        var fromState = instance.State;
        var toState = nextStep.IsEnd
            ? ProcessInstanceState.Completed
            : WorkflowStateResolver.DetermineNewState((WorkflowAction)workflowAction, instance.State);

        var history = ProcessInstanceFactory.CreateHistory(
            id,
            currentStep,
            (WorkflowAction)workflowAction,
            dto?.PerformedByUserId ?? instance.CreatedByUserId,
            comments,
            fromState,
            toState);

        currentStep.State = ProcessInstanceStepState.Completed;
        currentStep.CompletedAt = DateTime.UtcNow;
        currentStep.Comments = comments;
        currentStep.UpdatedAt = DateTime.UtcNow;

        instance.State = toState;
        instance.CompletedAt = nextStep.IsEnd ? DateTime.UtcNow : null;
        instance.UpdatedAt = DateTime.UtcNow;

        if (!nextStep.IsEnd)
        {
            var nextInstanceStep = ProcessInstanceFactory.CreateActiveStep(id, nextStep.Id, instance.CreatedByUserId);

            await _instanceStepRepository.AddAsync(nextInstanceStep);
        }

        await _historyRepository.AddAsync(history);
        await _instanceStepRepository.UpdateAsync(currentStep);
        await _instanceRepository.UpdateAsync(instance);

        var updatedInstance = await _instanceRepository.GetWithStepsAsync(id);
        var instanceData = await _dataService.GetAsync(id);
        return Ok(ProcessInstanceMapper.ToDetailDto(updatedInstance, instanceData));
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
        return Ok(ProcessInstanceMapper.ToSummaryDto(instance));
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

}
