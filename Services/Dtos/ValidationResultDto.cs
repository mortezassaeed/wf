namespace Services.Dtos;

public class ValidationResultDto
{
    public bool IsValid { get; set; }
    public List<ValidationErrorDto> Errors { get; set; } = new();

    public static ValidationResultDto Success()
    {
        return new ValidationResultDto { IsValid = true };
    }

    public static ValidationResultDto Failure(List<ValidationErrorDto> errors)
    {
        return new ValidationResultDto
        {
            IsValid = false,
            Errors = errors
        };
    }

    public void AddError(string field, string message)
    {
        Errors.Add(new ValidationErrorDto
        {
            Field = field,
            Message = message
        });
        IsValid = false;
    }
}
