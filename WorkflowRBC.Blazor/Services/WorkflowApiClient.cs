using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WorkflowRBC.Blazor.Models;

namespace WorkflowRBC.Blazor.Services;

public sealed class WorkflowApiClient
{
    private readonly HttpClient _httpClient;

    public WorkflowApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<List<ProcessDto>>> GetProcessesAsync()
    {
        return await SendAsync<List<ProcessDto>>(() => _httpClient.GetAsync("api/Processes"));
    }

    public async Task<ApiResult<List<ProcessDataTypeDto>>> GetProcessDataTypesAsync()
    {
        return await SendAsync<List<ProcessDataTypeDto>>(() => _httpClient.GetAsync("api/Processes/data-types"));
    }

    public async Task<ApiResult<ProcessDto>> GetProcessAsync(int processId)
    {
        return await SendAsync<ProcessDto>(() => _httpClient.GetAsync($"api/Processes/{processId}"));
    }

    public async Task<ApiResult<List<ProcessDataTypeDto>>> GetProcessAllowedDataTypesAsync(int processId)
    {
        return await SendAsync<List<ProcessDataTypeDto>>(() => _httpClient.GetAsync($"api/Processes/{processId}/data-types"));
    }

    public async Task<ApiResult<ProcessDto>> CreateProcessAsync(CreateProcessDto dto)
    {
        return await SendAsync<ProcessDto>(() => _httpClient.PostAsJsonAsync("api/Processes", dto));
    }

    public async Task<ApiResult<ProcessStepActionDto>> CreateProcessActionAsync(int processId, CreateProcessStepActionDto dto)
    {
        return await SendAsync<ProcessStepActionDto>(() => _httpClient.PostAsJsonAsync($"api/processes/{processId}/actions", dto));
    }

    public async Task<ApiResult<List<ProcessInstanceDto>>> GetInstancesAsync()
    {
        return await SendAsync<List<ProcessInstanceDto>>(() => _httpClient.GetAsync("api/ProcessInstances"));
    }

    public async Task<ApiResult<List<ProcessInstanceDto>>> GetCompletedInstancesAsync()
    {
        return await SendAsync<List<ProcessInstanceDto>>(() => _httpClient.GetAsync($"api/ProcessInstances?state={(int)ProcessInstanceState.Completed}"));
    }

    public async Task<ApiResult<List<CartableItemDto>>> GetCartableAsync(int userId)
    {
        return await SendAsync<List<CartableItemDto>>(() => _httpClient.GetAsync($"api/ProcessInstances/cartable?userId={userId}"));
    }

    public async Task<ApiResult<ProcessInstanceDto>> GetInstanceAsync(int instanceId)
    {
        return await SendAsync<ProcessInstanceDto>(() => _httpClient.GetAsync($"api/ProcessInstances/{instanceId}"));
    }

    public async Task<ApiResult<ProcessInstanceDto>> CreateInstanceAsync(CreateProcessInstanceRequest dto)
    {
        return await SendAsync<ProcessInstanceDto>(() => _httpClient.PostAsJsonAsync("api/ProcessInstances", dto));
    }

    public async Task<ApiResult<LeaveRequestDataDto>> GetLeaveRequestDataAsync(int instanceId)
    {
        return await SendAsync<LeaveRequestDataDto>(() => _httpClient.GetAsync($"api/ProcessInstances/{instanceId}/data"));
    }

    public async Task<ApiResult<string>> GetInstanceDataJsonAsync(int instanceId)
    {
        return await SendStringAsync(() => _httpClient.GetAsync($"api/ProcessInstances/{instanceId}/data"));
    }

    public async Task<ApiResult<LeaveRequestDataDto>> UpdateLeaveRequestDataAsync(int instanceId, LeaveRequestDataDto data, int userId)
    {
        return await UpdateProcessDataAsync<LeaveRequestDataDto>(instanceId, data.DataType, data, userId);
    }

    public async Task<ApiResult<PurchaseRequestDataDto>> UpdatePurchaseRequestDataAsync(int instanceId, PurchaseRequestDataDto data, int userId)
    {
        return await UpdateProcessDataAsync<PurchaseRequestDataDto>(instanceId, data.DataType, data, userId);
    }

    public async Task<ApiResult<T>> UpdateProcessDataAsync<T>(int instanceId, string dataType, object data, int userId)
    {
        var request = new ProcessInstanceDataRequest
        {
            DataType = dataType,
            Data = data,
            UserId = userId
        };

        return await SendAsync<T>(() => _httpClient.PutAsJsonAsync($"api/ProcessInstances/{instanceId}/data", request));
    }

    public async Task<ApiResult<ProcessInstanceDto>> ExecuteActionAsync(int instanceId, WorkflowAction action, string comments, int userId)
    {
        var request = new ExecuteActionRequest
        {
            ProcessInstanceId = instanceId,
            Action = action,
            PerformedByUserId = userId,
            Comments = comments
        };

        return await SendAsync<ProcessInstanceDto>(() => _httpClient.PostAsJsonAsync($"api/ProcessInstances/{instanceId}/execute/{(int)action}", request));
    }

    private static async Task<ApiResult<T>> SendAsync<T>(Func<Task<HttpResponseMessage>> request)
    {
        try
        {
            using var response = await request();
            if (response.StatusCode == HttpStatusCode.NoContent)
                return ApiResult<T>.Failure("The API returned no content.");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return ApiResult<T>.Failure(string.IsNullOrWhiteSpace(error)
                    ? $"{(int)response.StatusCode} {response.ReasonPhrase}"
                    : error);
            }

            var value = await response.Content.ReadFromJsonAsync<T>();
            return value is null
                ? ApiResult<T>.Failure("The API returned an empty response.")
                : ApiResult<T>.Success(value);
        }
        catch (Exception ex)
        {
            return ApiResult<T>.Failure(ex.Message);
        }
    }

    private static async Task<ApiResult<string>> SendStringAsync(Func<Task<HttpResponseMessage>> request)
    {
        try
        {
            using var response = await request();
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return ApiResult<string>.Failure(string.IsNullOrWhiteSpace(error)
                    ? $"{(int)response.StatusCode} {response.ReasonPhrase}"
                    : error);
            }

            var value = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(value)
                ? ApiResult<string>.Failure("The API returned an empty response.")
                : ApiResult<string>.Success(value);
        }
        catch (Exception ex)
        {
            return ApiResult<string>.Failure(ex.Message);
        }
    }
}
