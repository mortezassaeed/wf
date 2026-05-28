using Services.Dtos;

namespace Services.Resolver;

public interface IProcessDataTypeProvider
{
    IReadOnlyList<ProcessDataTypeDto> GetAll();
    bool Exists(string dataType);
}
