namespace Services.Dtos;

public class ResultDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ResultDto<T> SuccessResult(T data, string message = "Operation successful")
    {
        return new ResultDto<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ResultDto<T> FailureResult(string message, List<string> errors = null)
    {
        return new ResultDto<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}
