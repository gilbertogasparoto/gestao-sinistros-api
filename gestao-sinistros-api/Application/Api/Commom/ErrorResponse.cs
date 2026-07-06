namespace gestao_sinistros_api.Application.Api.Commom;

public class ErrorResponse
{
    public bool Success => false;

    public int StatusCode { get; set; }

    public string Message { get; set; } = string.Empty;

    public object? Errors { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}