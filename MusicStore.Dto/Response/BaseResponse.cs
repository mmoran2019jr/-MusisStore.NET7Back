namespace MusicStore.Dto.Response;

public class BaseResponse
{
    public bool Success { get; set; }

    public string? ErrorMessage { get; set; } 

    public string? Token { get; set; }
}