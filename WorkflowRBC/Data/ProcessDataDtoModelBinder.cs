using Microsoft.AspNetCore.Mvc.ModelBinding;
using Services.Dtos;
using Services.Resolver;
using System.Text.Json;

namespace WorkflowRBC.Data;

public class ProcessDataDtoModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);
        var body = await reader.ReadToEndAsync();
        var root = JsonDocument.Parse(body).RootElement;

        var dataType = root.GetProperty("dataType").GetString()
            ?? throw new InvalidOperationException("dataType is required.");
        var dataJson = root.GetProperty("data").GetRawText();
        var dataService = bindingContext.HttpContext.RequestServices.GetRequiredService<IProcessInstanceDataService>();

        var dto = new CreateProcessInstanceDto
        {
            ProcessCode = root.GetProperty("processCode").GetString()
                ?? throw new InvalidOperationException("processCode is required."),
            Title = root.GetProperty("title").GetString()!,
            Data = dataService.Deserialize(dataType, dataJson)
        };

        bindingContext.Result = ModelBindingResult.Success(dto);
    }
}
