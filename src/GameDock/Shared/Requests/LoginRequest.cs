using System.ComponentModel.DataAnnotations;

namespace GameDock.Shared.Requests;

public class LoginRequest
{
    [Required] public string Login { get; set; }
    [Required] public string Password { get; set; }
}