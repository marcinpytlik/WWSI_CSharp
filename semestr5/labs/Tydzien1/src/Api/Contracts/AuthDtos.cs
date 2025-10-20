using System.ComponentModel.DataAnnotations;

namespace Api.Contracts
{
    public class LoginRequest
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }

    public record LoginResponse(string Token);
}
