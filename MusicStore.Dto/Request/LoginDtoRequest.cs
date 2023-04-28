using System.ComponentModel.DataAnnotations;

namespace MusicStore.Dto.Request;

public class LoginDtoRequest
{
    [Required]
    public string UserName { get; set; } = default!;

    [Required]
    [MinLength(5)]
    public string Password { get; set; } = default!;

}