using DataAccess.Entities;
using Services.Dtos;
using System.Text.Json;

namespace Services.Resolver;

public abstract class ProcessDataHandlerBase<TDto, TEntity> : IProcessDataHandler
    where TDto : IProcessDataDto
    where TEntity : ProcessInstanceDataBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public abstract string DataType { get; }

    public IProcessDataDto Deserialize(string dataJson)
    {
        return JsonSerializer.Deserialize<TDto>(dataJson, JsonOptions)
            ?? throw new InvalidOperationException($"Invalid {DataType} data.");
    }

    public ProcessInstanceDataBase CreateEntity(IProcessDataDto dto)
    {
        if (dto is not TDto typedDto)
            throw new InvalidOperationException($"Expected DTO type {typeof(TDto).Name} for data type {DataType}.");

        return CreateEntity(typedDto);
    }

    public IProcessDataDto ToDto(ProcessInstanceDataBase entity)
    {
        if (entity is not TEntity typedEntity)
            throw new InvalidOperationException($"Expected entity type {typeof(TEntity).Name} for data type {DataType}.");

        return ToDto(typedEntity);
    }

    protected abstract TEntity CreateEntity(TDto dto);
    protected abstract IProcessDataDto ToDto(TEntity entity);
}
