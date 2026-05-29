using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Services.Dtos;

namespace Services.Resolver;

public class ProcessInstanceDataService : IProcessInstanceDataService
{
    private readonly WorkflowDbContext _context;
    private readonly IReadOnlyDictionary<string, IProcessDataHandler> _handlers;

    public ProcessInstanceDataService(WorkflowDbContext context, IEnumerable<IProcessDataHandler> handlers)
    {
        _context = context;
        _handlers = handlers.ToDictionary(h => h.DataType, StringComparer.OrdinalIgnoreCase);
    }

    public IProcessDataDto Deserialize(string dataType, string dataJson)
    {
        return GetHandler(dataType).Deserialize(dataJson);
    }

    public ProcessInstanceDataBase? CreateEntity(IProcessDataDto? dto, int userId)
    {
        if (dto == null)
            return null;

        var entity = GetHandler(dto.DataType).CreateEntity(dto);

        SetCommonFields(entity, dto.DataType, userId, isNew: true);
        return entity;
    }

    public async Task<IProcessDataDto?> GetAsync(int processInstanceId)
    {
        var data = await _context.ProcessInstanceDataBase
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.ProcessInstanceId == processInstanceId);

        if (data == null)
            return null;

        return GetHandler(data.DataType).ToDto(data);
    }

    public async Task<IProcessDataDto> UpdateAsync(int processInstanceId, IProcessDataDto dto, int userId)
    {
        var existing = await _context.ProcessInstanceDataBase
            .FirstOrDefaultAsync(d => d.ProcessInstanceId == processInstanceId);

        if (existing == null)
            throw new InvalidOperationException($"No data found for process instance {processInstanceId}.");

        _context.ProcessInstanceDataBase.Remove(existing);
        await _context.SaveChangesAsync();

        var entity = CreateEntity(dto, userId)
            ?? throw new InvalidOperationException("Process instance data is required.");

        entity.ProcessInstanceId = processInstanceId;
        await _context.ProcessInstanceDataBase.AddAsync(entity);
        await _context.SaveChangesAsync();

        return await GetAsync(processInstanceId)
            ?? throw new InvalidOperationException("Process instance data was saved but could not be loaded.");
    }

    private IProcessDataHandler GetHandler(string dataType)
    {
        if (_handlers.TryGetValue(dataType, out var handler))
            return handler;

        throw new NotSupportedException($"Unknown data type: {dataType}");
    }

    private static void SetCommonFields(ProcessInstanceDataBase entity, string dataType, int userId, bool isNew)
    {
        entity.DataType = dataType;

        if (isNew)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedByUserId = userId;
        }
        else
        {
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = userId;
        }
    }
}
