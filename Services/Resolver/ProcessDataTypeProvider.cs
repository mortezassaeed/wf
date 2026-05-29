using DataAccess.Entities;
using Services.Dtos;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Services.Resolver;

public class ProcessDataTypeProvider : IProcessDataTypeProvider
{
    // Base process data fields are managed by the engine, so clients only receive workflow-specific payload fields.
    private static readonly HashSet<string> ExcludedFieldNames = new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(ProcessInstanceDataBaseDto.Id),
        nameof(ProcessInstanceDataBaseDto.ProcessInstanceId),
        nameof(ProcessInstanceDataBaseDto.DataType),
        nameof(ProcessInstanceDataBaseDto.CreatedAt),
        nameof(ProcessInstanceDataBaseDto.UpdatedAt)
    };

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
        // DTOs are the public API contract; entities are only used to enrich metadata with configured data type names.
        var entityMetadata = typeof(ProcessInstanceDataBase).Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ProcessInstanceDataBase)))
            .Select(CreateEntityMetadata)
            .ToDictionary(type => type.Code, StringComparer.OrdinalIgnoreCase);

        return typeof(ProcessInstanceDataBaseDto).Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ProcessInstanceDataBaseDto)))
            .Select(dtoType => CreateDataType(dtoType, entityMetadata))
            .OrderBy(type => type.DisplayName)
            .ToList();
    }

    private static ProcessDataTypeDto CreateEntityMetadata(Type entityType)
    {
        var attribute = entityType.GetCustomAttribute<ProcessDataTypeAttribute>();
        return new ProcessDataTypeDto
        {
            Code = attribute?.Code ?? entityType.Name,
            DisplayName = attribute?.DisplayName ?? entityType.Name,
            EntityType = entityType.Name
        };
    }

    private static ProcessDataTypeDto CreateDataType(
        Type dtoType,
        IReadOnlyDictionary<string, ProcessDataTypeDto> entityMetadata)
    {
        var code = ResolveDataTypeCode(dtoType);
        entityMetadata.TryGetValue(code, out var entity);

        return new ProcessDataTypeDto
        {
            Code = code,
            DisplayName = entity?.DisplayName ?? ToDisplayName(dtoType.Name),
            EntityType = entity?.EntityType ?? string.Empty,
            DtoType = dtoType.Name,
            Fields = DiscoverFields(dtoType)
        };
    }

    private static string ResolveDataTypeCode(Type dtoType)
    {
        // Keep the same code used by runtime payload mapping, for example LEAVE_REQUEST.
        if (Activator.CreateInstance(dtoType) is IProcessDataDto dto &&
            !string.IsNullOrWhiteSpace(dto.DataType))
        {
            return dto.DataType;
        }

        return dtoType.Name;
    }

    private static List<ProcessDataFieldDto> DiscoverFields(Type dtoType)
    {
        var nullabilityContext = new NullabilityInfoContext();

        return dtoType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetMethod is not null)
            .Where(property => !ExcludedFieldNames.Contains(property.Name))
            .Select((property, index) => CreateField(property, nullabilityContext, index))
            .Where(field => field is not null)
            .Select(field => field!)
            .OrderBy(field => field.Order)
            .ThenBy(field => field.DisplayName)
            .ToList();
    }

    private static ProcessDataFieldDto? CreateField(
        PropertyInfo property,
        NullabilityInfoContext nullabilityContext,
        int index)
    {
        // Attributes are optional; without them the schema still falls back to property names and CLR types.
        var fieldAttribute = property.GetCustomAttribute<ProcessDataFieldAttribute>();
        if (fieldAttribute?.Hidden == true)
            return null;

        var displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
        var maxLengthAttribute = property.GetCustomAttribute<MaxLengthAttribute>();
        var stringLengthAttribute = property.GetCustomAttribute<StringLengthAttribute>();
        var rangeAttribute = property.GetCustomAttribute<RangeAttribute>();

        var customOrder = fieldAttribute?.Order ?? 0;
        var order = customOrder > 0
            ? customOrder
            : displayAttribute?.GetOrder() ?? ((index + 1) * 10);

        return new ProcessDataFieldDto
        {
            Name = ToCamelCase(property.Name),
            DisplayName = fieldAttribute?.Label
                ?? displayAttribute?.GetName()
                ?? ToDisplayName(property.Name),
            Type = ToSchemaType(property.PropertyType),
            Required = IsRequired(property, nullabilityContext),
            MaxLength = maxLengthAttribute?.Length ?? stringLengthAttribute?.MaximumLength,
            Minimum = ConvertToNullableDouble(rangeAttribute?.Minimum),
            Maximum = ConvertToNullableDouble(rangeAttribute?.Maximum),
            HelpText = fieldAttribute?.HelpText ?? displayAttribute?.GetDescription(),
            Placeholder = fieldAttribute?.Placeholder,
            ControlType = fieldAttribute?.ControlType,
            Order = order,
            Options = GetOptions(property.PropertyType)
        };
    }

    private static bool IsRequired(PropertyInfo property, NullabilityInfoContext nullabilityContext)
    {
        if (property.GetCustomAttribute<RequiredAttribute>() is not null)
            return true;

        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        if (propertyType.IsValueType)
            return Nullable.GetUnderlyingType(property.PropertyType) is null;

        return nullabilityContext.Create(property).WriteState == NullabilityState.NotNull;
    }

    private static string ToSchemaType(Type type)
    {
        var propertyType = Nullable.GetUnderlyingType(type) ?? type;

        if (propertyType == typeof(string))
            return "string";

        if (propertyType == typeof(DateTime) || propertyType == typeof(DateOnly))
            return "date";

        if (propertyType == typeof(bool))
            return "boolean";

        if (propertyType.IsEnum)
            return "enum";

        if (propertyType == typeof(int) ||
            propertyType == typeof(long) ||
            propertyType == typeof(short) ||
            propertyType == typeof(decimal) ||
            propertyType == typeof(double) ||
            propertyType == typeof(float))
        {
            return "number";
        }

        return "object";
    }

    private static List<string> GetOptions(Type type)
    {
        var propertyType = Nullable.GetUnderlyingType(type) ?? type;
        return propertyType.IsEnum
            ? Enum.GetNames(propertyType).ToList()
            : new List<string>();
    }

    private static double? ConvertToNullableDouble(object? value)
    {
        if (value is null)
            return null;

        return double.TryParse(value.ToString(), out var result) ? result : null;
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || char.IsLower(value[0]))
            return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    private static string ToDisplayName(string value)
    {
        var name = value.EndsWith("Dto", StringComparison.OrdinalIgnoreCase)
            ? value[..^3]
            : value;

        return string.Concat(name.Select((character, index) =>
            index > 0 && char.IsUpper(character)
                ? " " + character
                : character.ToString()));
    }
}
