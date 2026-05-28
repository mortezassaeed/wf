using DataAccess.Entities;
using Services.Dtos;

namespace Services.Resolver;

public interface IProcessInstanceDataService
{
    ProcessInstanceDataBase? CreateEntity(IProcessDataDto? dto, int userId);
    IProcessDataDto Deserialize(string dataType, string dataJson);
    Task<IProcessDataDto?> GetAsync(int processInstanceId);
    Task<IProcessDataDto> UpsertAsync(int processInstanceId, IProcessDataDto dto, int userId);
}
