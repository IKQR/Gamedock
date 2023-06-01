using Microsoft.AspNetCore.Identity;

namespace GameDock.Server.Infrastructure.Entities.Identity;

public sealed class AppUser : IdentityUser
{
    public AppUser() : base()
    {
    }

    public AppUser(string userName) : base(userName)
    {
        NormalizedUserName = userName.ToUpperInvariant();
    }
}