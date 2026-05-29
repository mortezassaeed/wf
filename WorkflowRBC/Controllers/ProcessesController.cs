// WorkflowEngine.WebAPI/Controllers/ProcessController.cs
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Dtos;
using Services.Factories;
using Services.Mappers;
using Services.Resolver;

namespace WorkflowEngine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessesController : ControllerBase
{
    private readonly IProcessRepository _processRepository;
    private readonly IProcessDataTypeProvider _dataTypeProvider;

    public ProcessesController(
        IProcessRepository processRepository,
        IProcessDataTypeProvider dataTypeProvider)
    {
        _processRepository = processRepository;
        _dataTypeProvider = dataTypeProvider;
    }

    [HttpGet("data-types")]
    public ActionResult<IEnumerable<ProcessDataTypeDto>> GetDataTypes()
    {
        return Ok(_dataTypeProvider.GetAll());
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProcessDto>>> GetAll()
    {
        var processes = await _processRepository.GetAllAsync();
        var dtos = processes.Select(ProcessMapper.ToDto);
        return Ok(dtos);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<ProcessDto>>> GetActive()
    {
        var processes = await _processRepository.GetActiveProcessesAsync();
        var dtos = processes.Select(ProcessMapper.ToDto);
        return Ok(dtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProcessDto>> GetById(int id)
    {
        var process = await _processRepository.GetWithStepsAsync(id);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {id} not found" });

        return Ok(ProcessMapper.ToDto(process));
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<ProcessDto>> GetByCode(string code)
    {
        var process = await _processRepository.GetByCodeAsync(code);
        if (process == null)
            return NotFound(new { Message = $"Process with code '{code}' not found" });

        return Ok(ProcessMapper.ToDto(process));
    }

    [HttpGet("{id:int}/definition")]
    public async Task<ActionResult<WorkflowDefinitionDto>> GetDefinition(int id)
    {
        var process = await _processRepository.GetWithStepsAsync(id);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {id} not found" });

        return Ok(ProcessMapper.ToWorkflowDefinitionDto(process));
    }

    [HttpGet("{id:int}/data-types")]
    public async Task<ActionResult<IEnumerable<ProcessDataTypeDto>>> GetAllowedDataTypes(int id)
    {
        var process = await _processRepository.GetWithStepsAsync(id);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {id} not found" });

        var allowedCodes = process.ProcessAllowedDataTypes
            .Select(t => t.DataType)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return Ok(_dataTypeProvider.GetAll().Where(t => allowedCodes.Contains(t.Code)));
    }

    [HttpPut("{id:int}/data-types")]
    public async Task<ActionResult<ProcessDto>> UpdateAllowedDataTypes(
        int id,
        [FromBody] ProcessAllowedDataTypesRequestDto dto)
    {
        var process = await _processRepository.GetWithStepsAsync(id);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {id} not found" });

        var validationError = ValidateAllowedDataTypes(dto.DataTypes);
        if (validationError != null)
            return BadRequest(new { Message = validationError });

        SetAllowedDataTypes(process, dto.DataTypes);
        process.UpdatedAt = DateTime.UtcNow;

        await _processRepository.UpdateAsync(process);
        return Ok(ProcessMapper.ToDto(process));
    }

    [HttpPost]
    public async Task<ActionResult<ProcessDto>> Create([FromBody] CreateProcessDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
            return BadRequest(new { Message = "Process code is required." });

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { Message = "Process name is required." });

        var validationError = ValidateAllowedDataTypes(dto.AllowedDataTypes);
        if (validationError != null)
            return BadRequest(new { Message = validationError });

        var process = ProcessFactory.CreateProcess(dto, NormalizeDataTypes(dto.AllowedDataTypes));

        await _processRepository.AddAsync(process);
        return CreatedAtAction(nameof(GetById), new { id = process.Id }, ProcessMapper.ToDto(process));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProcessDto>> Update(int id, [FromBody] UpdateProcessDto dto)
    {
        var process = await _processRepository.GetWithStepsAsync(id);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {id} not found" });

        var validationError = ValidateAllowedDataTypes(dto.AllowedDataTypes);
        if (validationError != null)
            return BadRequest(new { Message = validationError });

        process.Name = dto.Name;
        process.Description = dto.Description;
        SetAllowedDataTypes(process, dto.AllowedDataTypes);
        process.IsActive = dto.IsActive;
        process.UpdatedAt = DateTime.UtcNow;

        await _processRepository.UpdateAsync(process);
        return Ok(ProcessMapper.ToDto(process));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var process = await _processRepository.GetByIdAsync(id);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {id} not found" });

        await _processRepository.DeleteAsync(id);
        return NoContent();
    }

    private string? ValidateAllowedDataTypes(IEnumerable<string>? dataTypes)
    {
        var selectedDataTypes = NormalizeDataTypes(dataTypes).ToList();
        if (selectedDataTypes.Count == 0)
            return "At least one process data type is required.";

        var invalidDataType = selectedDataTypes.FirstOrDefault(dataType => !_dataTypeProvider.Exists(dataType));
        return invalidDataType == null ? null : $"Unknown process data type: {invalidDataType}";
    }

    private static void SetAllowedDataTypes(Process process, IEnumerable<string> dataTypes)
    {
        process.ProcessAllowedDataTypes.Clear();
        foreach (var dataType in NormalizeDataTypes(dataTypes))
        {
            process.ProcessAllowedDataTypes.Add(new ProcessAllowedDataType
            {
                ProcessId = process.Id,
                DataType = dataType
            });
        }
    }

    private static IEnumerable<string> NormalizeDataTypes(IEnumerable<string>? dataTypes)
    {
        return (dataTypes ?? Enumerable.Empty<string>())
            .Where(dataType => !string.IsNullOrWhiteSpace(dataType))
            .Select(dataType => dataType.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }

}



[ApiController]
[Route("api/processes/{processId:int}/steps")]
public class ProcessStepsController : ControllerBase
{
    private readonly IRepository<ProcessStep> _stepRepository;
    private readonly IProcessRepository _processRepository;

    public ProcessStepsController(
        IRepository<ProcessStep> stepRepository,
        IProcessRepository processRepository)
    {
        _stepRepository = stepRepository;
        _processRepository = processRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProcessStepDto>>> GetSteps(int processId)
    {
        var process = await _processRepository.GetWithStepsAsync(processId);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {processId} not found" });

        var dtos = process.ProcessSteps.Select(ProcessMapper.ToStepDto);
        return Ok(dtos);
    }

    [HttpGet("{stepId:int}")]
    public async Task<ActionResult<ProcessStepDto>> GetStep(int processId, int stepId)
    {
        var step = await _stepRepository.GetByIdAsync(stepId);
        if (step == null || step.ProcessId != processId)
            return NotFound(new { Message = $"Step with ID {stepId} not found in process {processId}" });

        return Ok(ProcessMapper.ToStepDto(step));
    }

    [HttpPost]
    public async Task<ActionResult<ProcessStepDto>> CreateStep(int processId, [FromBody] CreateProcessStepDto dto)
    {
        var process = await _processRepository.GetByIdAsync(processId);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {processId} not found" });

        var step = ProcessFactory.CreateStep(processId, dto);

        await _stepRepository.AddAsync(step);
        return CreatedAtAction(nameof(GetStep), new { processId, stepId = step.Id }, ProcessMapper.ToStepDto(step));
    }

    [HttpPut("{stepId:int}")]
    public async Task<ActionResult<ProcessStepDto>> UpdateStep(int processId, int stepId, [FromBody] CreateProcessStepDto dto)
    {
        var step = await _stepRepository.GetByIdAsync(stepId);
        if (step == null || step.ProcessId != processId)
            return NotFound(new { Message = $"Step with ID {stepId} not found in process {processId}" });

        step.Name = dto.Name;
        step.Description = dto.Description ?? string.Empty;
        step.Order = dto.Order;
        step.IsStart = dto.IsStart;
        step.IsEnd = dto.IsEnd;
        step.RequiresApproval = dto.RequiresApproval;
        step.ApproverRoles = dto.ApproverRoles ?? string.Empty;
        step.IsActive = dto.IsActive;
        step.UpdatedAt = DateTime.UtcNow;

        await _stepRepository.UpdateAsync(step);
        return Ok(ProcessMapper.ToStepDto(step));
    }

    [HttpDelete("{stepId:int}")]
    public async Task<ActionResult> DeleteStep(int processId, int stepId)
    {
        var step = await _stepRepository.GetByIdAsync(stepId);
        if (step == null || step.ProcessId != processId)
            return NotFound(new { Message = $"Step with ID {stepId} not found in process {processId}" });

        await _stepRepository.DeleteAsync(step.Id);
        return NoContent();
    }

}

[ApiController]
[Route("api/processes/{processId:int}/actions")]
public class ProcessStepActionsController : ControllerBase
{
    private readonly IRepository<ProcessStepAction> _actionRepository;
    private readonly IProcessRepository _processRepository;

    public ProcessStepActionsController(
        IRepository<ProcessStepAction> actionRepository,
        IProcessRepository processRepository)
    {
        _actionRepository = actionRepository;
        _processRepository = processRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProcessStepActionDto>>> GetActions(int processId)
    {
        var process = await _processRepository.GetWithStepsAsync(processId);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {processId} not found" });

        var dtos = process.ProcessStepActions?.Select(ProcessMapper.ToActionDto) ?? Enumerable.Empty<ProcessStepActionDto>();
        return Ok(dtos);
    }

    [HttpPost]
    public async Task<ActionResult<ProcessStepActionDto>> CreateAction(int processId, [FromBody] CreateProcessStepActionDto dto)
    {
        if (dto.ProcessId != 0 && dto.ProcessId != processId)
            return BadRequest(new { Message = "Route process ID and body process ID do not match" });

        var process = await _processRepository.GetWithStepsAsync(processId);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {processId} not found" });

        var fromStepExists = process.ProcessSteps.Any(s => s.Id == dto.FromStepId);
        var toStepExists = process.ProcessSteps.Any(s => s.Id == dto.ToStepId);
        if (!fromStepExists || !toStepExists)
            return BadRequest(new { Message = "FromStepId and ToStepId must both belong to the process" });

        var action = ProcessFactory.CreateAction(processId, dto);

        await _actionRepository.AddAsync(action);
        return CreatedAtAction(nameof(GetActions), new { processId }, ProcessMapper.ToActionDto(action));
    }

    [HttpDelete("{actionId:int}")]
    public async Task<ActionResult> DeleteAction(int processId, int actionId)
    {
        var action = await _actionRepository.GetByIdAsync(actionId);
        if (action == null || action.ProcessId != processId)
            return NotFound(new { Message = $"Action with ID {actionId} not found in process {processId}" });

        await _actionRepository.DeleteAsync(actionId);
        return NoContent();
    }

}
