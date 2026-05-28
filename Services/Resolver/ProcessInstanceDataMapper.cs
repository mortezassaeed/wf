using Services.Dtos;
using System.Text.Json;

namespace Services.Resolver;

public static class ProcessInstanceDataMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static IProcessDataDto Deserialize(string dataType, string dataJson)
    {
        return dataType switch
        {
            "LEAVE_REQUEST" => JsonSerializer.Deserialize<LeaveRequestDto>(dataJson, JsonOptions)
                ?? throw new InvalidOperationException("Invalid leave request data."),
            "PURCHASE_REQUEST" => JsonSerializer.Deserialize<PurchaseRequestDto>(dataJson, JsonOptions)
                ?? throw new InvalidOperationException("Invalid purchase request data."),
            _ => throw new NotSupportedException($"Unknown data type: {dataType}")
        };
    }
}
