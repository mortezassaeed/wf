namespace DataAccess.Entities;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ProcessDataTypeAttribute : Attribute
{
    public ProcessDataTypeAttribute(string code, string displayName)
    {
        Code = code;
        DisplayName = displayName;
    }

    public string Code { get; }
    public string DisplayName { get; }
}
