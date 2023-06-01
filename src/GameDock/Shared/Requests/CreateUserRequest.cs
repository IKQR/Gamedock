using System.ComponentModel.DataAnnotations;

namespace GameDock.Shared.Requests;

public class CreateUserRequest
{
    [Required]
    public string Login { get; set; }
}