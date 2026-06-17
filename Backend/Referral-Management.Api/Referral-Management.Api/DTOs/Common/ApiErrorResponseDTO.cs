namespace Referral_Management.Api.DTOs.Common;

public class ApiErrorResponseDTO
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public DateTime Timestamp { get; set; }
}