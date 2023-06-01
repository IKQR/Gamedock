namespace GameDock.Shared.Responses;

public class CreateUserResponse
{
    
    public string Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string[] Roles { get; set; }
}