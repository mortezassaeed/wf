using Microsoft.AspNetCore.Mvc;
using DataAccess.Enums;
using Services;
using Services.Dtos;
using Services.Resolver;

namespace WorkflowEngine.Controllers;

[ApiController]
[Route("api/ProcessInstances/{processInstanceId:int}/data")]
public class ProcessInstanceDataController : ControllerBase
{
    private readonly IProcessInstanceRepository _instanceRepository;
    private readonly IProcessInstanceDataService _dataService;

    public ProcessInstanceDataController(
        IProcessInstanceRepository instanceRepository,
        IProcessInstanceDataService dataService)
    {
        _instanceRepository = instanceRepository;
        _dataService = dataService;
    }

    [HttpGet]
    public async Task<ActionResult<IProcessDataDto>> GetData(int processInstanceId)
    {
        var instance = await _instanceRepository.GetByIdAsync(processInstanceId);
        if (instance == null)
            return NotFound(new { Message = $"Process instance with ID {processInstanceId} not found" });

        var data = await _dataService.GetAsync(processInstanceId);
        if (data == null)
            return NotFound(new { Message = $"No data found for process instance {processInstanceId}" });

        return Ok(data);
    }

    [HttpPut]
    public async Task<ActionResult<IProcessDataDto>> UpdateData(
        int processInstanceId,
        [FromBody] ProcessInstanceDataRequestDto request)
    {
        var instance = await _instanceRepository.GetWithStepsAsync(processInstanceId);
        if (instance == null)
            return NotFound(new { Message = $"Process instance with ID {processInstanceId} not found" });

        var currentStep = instance.ProcessInstanceSteps?
            .FirstOrDefault(step => step.State == ProcessInstanceStepState.Active);

        if (currentStep == null)
            return BadRequest(new { Message = "No active step found for this process instance." });

        if (currentStep.ProcessStep?.CanEditData != true)
            return BadRequest(new { Message = "The current workflow step does not allow editing process data." });

        IProcessDataDto dto;
        try
        {
            dto = _dataService.Deserialize(request.DataType, request.Data.GetRawText());
        }
        catch (Exception ex) when (ex is InvalidOperationException or NotSupportedException or System.Text.Json.JsonException)
        {
            return BadRequest(new { ex.Message });
        }

        IProcessDataDto data;
        try
        {
            data = await _dataService.UpdateAsync(processInstanceId, dto, request.UserId);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { ex.Message });
        }

        return Ok(data);
    }
}
