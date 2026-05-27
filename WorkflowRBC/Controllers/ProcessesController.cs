// WorkflowEngine.WebAPI/Controllers/ProcessController.cs
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Dtos;

namespace WorkflowEngine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessesController : ControllerBase
{
    private readonly IProcessRepository _processRepository;

    public ProcessesController(IProcessRepository processRepository)
    {
        _processRepository = processRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProcessDto>>> GetAll()
    {
        var processes = await _processRepository.GetAllAsync();
        var dtos = processes.Select(MapToDto);
        return Ok(dtos);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<ProcessDto>>> GetActive()
    {
        var processes = await _processRepository.GetActiveProcessesAsync();
        var dtos = processes.Select(MapToDto);
        return Ok(dtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProcessDto>> GetById(int id)
    {
        var process = await _processRepository.GetWithStepsAsync(id);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {id} not found" });

        return Ok(MapToDetailDto(process));
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<ProcessDto>> GetByCode(string code)
    {
        var process = await _processRepository.GetByCodeAsync(code);
        if (process == null)
            return NotFound(new { Message = $"Process with code '{code}' not found" });

        return Ok(MapToDto(process));
    }

    [HttpGet("{id:int}/definition")]
    public async Task<ActionResult<WorkflowDefinitionDto>> GetDefinition(int id)
    {
        var process = await _processRepository.GetWithStepsAsync(id);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {id} not found" });

        return Ok(MapToWorkflowDefinition(process));
    }

    [HttpPost]
    public async Task<ActionResult<ProcessDto>> Create([FromBody] CreateProcessDto dto)
    {
        var process = new Process
        {
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            ProcessSteps = dto.Steps?.Select(s => new ProcessStep
            {
                Code = s.Code,
                Name = s.Name,
                Description = s.Description,
                Order = s.Order,
                IsStart = s.IsStart,
                IsEnd = s.IsEnd,
                RequiresApproval = s.RequiresApproval,
                ApproverRoles = s.ApproverRoles,
                IsActive = s.IsActive,
                CreatedAt = DateTime.UtcNow,
               
            }).ToList()
        };

        await _processRepository.AddAsync(process);
        return CreatedAtAction(nameof(GetById), new { id = process.Id }, MapToDto(process));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProcessDto>> Update(int id, [FromBody] UpdateProcessDto dto)
    {
        var process = await _processRepository.GetByIdAsync(id);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {id} not found" });

        process.Name = dto.Name;
        process.Description = dto.Description;
        process.IsActive = dto.IsActive;
        process.UpdatedAt = DateTime.UtcNow;

        await _processRepository.UpdateAsync(process);
        return Ok(MapToDto(process));
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

    private static ProcessDto MapToDto(Process process)
    {
        return new ProcessDto
        {
            Id = process.Id,
            Code = process.Code,
            Name = process.Name,
            Description = process.Description,
            IsActive = process.IsActive,
            CreatedAt = process.CreatedAt,
            UpdatedAt = process.UpdatedAt,
            Steps = process.ProcessSteps?.Select(s => new ProcessStepDto
            {
                Id = s.Id,
                ProcessId = s.ProcessId,
                Code = s.Code,
                Name = s.Name,
                Description = s.Description,
                Order = s.Order,
                IsStart = s.IsStart,
                IsEnd = s.IsEnd,
                RequiresApproval = s.RequiresApproval,
                ApproverRoles = s.ApproverRoles,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,

            }).ToList(),
        };
    }

    private static ProcessDto MapToDetailDto(Process process)
    {
        return new ProcessDto
        {
            Id = process.Id,
            Code = process.Code,
            Name = process.Name,
            Description = process.Description,
            IsActive = process.IsActive,
            CreatedAt = process.CreatedAt,
            UpdatedAt = process.UpdatedAt,
            Steps = process.ProcessSteps?.Select(s => new ProcessStepDto
            {
                Id = s.Id,
                ProcessId = s.ProcessId,
                Code = s.Code,
                Name = s.Name,
                Description = s.Description,
                Order = s.Order,
                IsStart = s.IsStart,
                IsEnd = s.IsEnd,
                RequiresApproval = s.RequiresApproval,
                ApproverRoles = s.ApproverRoles,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                
            }).ToList()
        };
    }

    private static WorkflowDefinitionDto MapToWorkflowDefinition(Process process)
    {
        return new WorkflowDefinitionDto
        {
            //ProcessId = process.Id,
            ProcessCode = process.Code,
            ProcessName = process.Name,
            Steps = process.ProcessSteps?.Select(s => new WorkflowStepDefinitionDto
            {
                //StepId = s.Id,
                //StepCode = s.Code,
                //StepName = s.Name,
                //Order = s.Order,
                IsStart = s.IsStart,
                IsEnd = s.IsEnd,
                RequiresApproval = s.RequiresApproval,
                //ApproverRoles = s.ApproverRoles,
                //AvailableActions = s.ProcessStepActions?.Select(a => a.Action).ToList()
            }).ToList()
        };
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

        var dtos = process.ProcessSteps.Select(MapToDto);
        return Ok(dtos);
    }

    [HttpGet("{stepId:int}")]
    public async Task<ActionResult<ProcessStepDto>> GetStep(int processId, int stepId)
    {
        var step = await _stepRepository.GetByIdAsync(stepId);
        if (step == null || step.ProcessId != processId)
            return NotFound(new { Message = $"Step with ID {stepId} not found in process {processId}" });

        return Ok(MapToDto(step));
    }

    [HttpPost]
    public async Task<ActionResult<ProcessStepDto>> CreateStep(int processId, [FromBody] CreateProcessStepDto dto)
    {
        var process = await _processRepository.GetByIdAsync(processId);
        if (process == null)
            return NotFound(new { Message = $"Process with ID {processId} not found" });

        var step = new ProcessStep
        {
            ProcessId = processId,
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            Order = dto.Order,
            IsStart = dto.IsStart,
            IsEnd = dto.IsEnd,
            RequiresApproval = dto.RequiresApproval,
            ApproverRoles = dto.ApproverRoles,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
            //ProcessStepActions = dto.Actions?.Select(a => new ProcessStepAction
            //{
            //    Action = a.Action,
            //    DisplayName = a.DisplayName,
            //    IsActive = a.IsActive,
            //    CreatedAt = DateTime.UtcNow
            //}).ToList()
        };

        await _stepRepository.AddAsync(step);
        return CreatedAtAction(nameof(GetStep), new { processId, stepId = step.Id }, MapToDto(step));
    }

    [HttpPut("{stepId:int}")]
    public async Task<ActionResult<ProcessStepDto>> UpdateStep(int processId, int stepId, [FromBody] CreateProcessStepDto dto)
    {
        var step = await _stepRepository.GetByIdAsync(stepId);
        if (step == null || step.ProcessId != processId)
            return NotFound(new { Message = $"Step with ID {stepId} not found in process {processId}" });

        step.Name = dto.Name;
        step.Description = dto.Description;
        step.Order = dto.Order;
        step.IsStart = dto.IsStart;
        step.IsEnd = dto.IsEnd;
        step.RequiresApproval = dto.RequiresApproval;
        step.ApproverRoles = dto.ApproverRoles;
        step.IsActive = dto.IsActive;
        step.UpdatedAt = DateTime.UtcNow;

        await _stepRepository.UpdateAsync(step);
        return Ok(MapToDto(step));
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

    private static ProcessStepDto MapToDto(ProcessStep step)
    {
        return new ProcessStepDto
        {
            Id = step.Id,
            ProcessId = step.ProcessId,
            Code = step.Code,
            Name = step.Name,
            Description = step.Description,
            Order = step.Order,
            IsStart = step.IsStart,
            IsEnd = step.IsEnd,
            RequiresApproval = step.RequiresApproval,
            ApproverRoles = step.ApproverRoles,
            IsActive = step.IsActive,
            CreatedAt = step.CreatedAt,
            UpdatedAt = step.UpdatedAt
            //Actions = step.ProcessStepActions?.Select(a => new ProcessStepActionDto
            //{
            //    Id = a.Id,
            //    ProcessStepId = a.ProcessStepId,
            //    Action = a.Action,
            //    DisplayName = a.DisplayName,
            //    IsActive = a.IsActive,
            //    CreatedAt = a.CreatedAt,
            //    UpdatedAt = a.UpdatedAt,
            //    FromStepId = a.FromStepId,
            //    ToStepId = a.ToStepId
            //}
            //).ToList()
        };
    }
}
