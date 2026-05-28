using DataAccess.Entities;
using Services.Dtos;
using System.Reflection;

namespace Services.Resolver;

public class ProcessDataTypeProvider : IProcessDataTypeProvider
{
    private static readonly Lazy<IReadOnlyList<ProcessDataTypeDto>> DataTypes = new(DiscoverDataTypes);

    public IReadOnlyList<ProcessDataTypeDto> GetAll()
    {
        return DataTypes.Value;
    }

    public bool Exists(string dataType)
    {
        if (string.IsNullOrWhiteSpace(dataType))
            return false;

        return DataTypes.Value.Any(t => t.Code.Equals(dataType, StringComparison.OrdinalIgnoreCase));
    }

    private static IReadOnlyList<ProcessDataTypeDto> DiscoverDataTypes()
    {
        return typeof(ProcessInstanceDataBase).Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ProcessInstanceDataBase)))
            .Select(type =>
            {
                var attribute = type.GetCustomAttribute<ProcessDataTypeAttribute>();
                return new ProcessDataTypeDto
                {
                    Code = attribute?.Code ?? type.Name,
                    DisplayName = attribute?.DisplayName ?? type.Name,
                    EntityType = type.Name
                };
            })
            .OrderBy(type => type.DisplayName)
            .ToList();
    }
}
