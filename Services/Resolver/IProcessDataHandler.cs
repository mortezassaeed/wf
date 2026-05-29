using DataAccess.Entities;
using Services.Dtos;

namespace Services.Resolver;

public interface IProcessDataHandler
{
    string DataType { get; }

    IProcessDataDto Deserialize(string dataJson);
    ProcessInstanceDataBase CreateEntity(IProcessDataDto dto);
    IProcessDataDto ToDto(ProcessInstanceDataBase entity);
}
