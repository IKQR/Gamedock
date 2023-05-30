using System.Linq;
using GameDock.Server.Infrastructure.Database;
using GameDock.Server.Infrastructure.Entities.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace GameDock.Server.Utils;

public static class Auth
{
    public static IServiceCollection ConfigureAuth(this IServiceCollection services)
    {
        services
            .AddDefaultIdentity<AppUser>(opt =>
            {
                opt.User.RequireUniqueEmail = false;

                opt.SignIn.RequireConfirmedEmail = false;
                opt.SignIn.RequireConfirmedAccount = false;
                opt.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<InfoDbContext>();

        services
            .AddIdentityServer()
            .AddApiAuthorization<AppUser, InfoDbContext>(opt =>
            {
                opt.IdentityResources["openid"].UserClaims.Add("role");
                opt.ApiResources.Single().UserClaims.Add("role");
            });

        services.AddAuthentication()
            .AddIdentityServerJwt();

        return services;
    }
}