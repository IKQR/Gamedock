using System;
using System.Text;
using GameDock.Server.Infrastructure.Database;
using GameDock.Server.Infrastructure.Entities.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace GameDock.Server.Utils;

public static class Auth
{
    public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptions = new AuthOptions(configuration);
        
        services.AddSingleton(authOptions);
        
        services.AddIdentityCore<AppUser>(opt =>
            {
                opt.User.RequireUniqueEmail = false;
                opt.SignIn.RequireConfirmedEmail = false;
                opt.SignIn.RequireConfirmedAccount = false;
                opt.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<InfoDbContext>();

        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = authOptions.Audience,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = authOptions.SymmetricSecurityKey,
                };
            });

        return services;
    }
}

public class AuthOptions
{
    public string Issuer { get; }
    public string Audience { get; }
    public TimeSpan Lifetime { get; }
    private string Secret { get; }

    public AuthOptions(IConfiguration configuration)
    {
        Issuer = configuration["JWT:Issuer"] ?? throw new ArgumentNullException("JWT:Issuer is required");
        Audience = configuration["JWT:Audience"] ?? throw new ArgumentNullException("JWT:Audience is required");
        Secret = configuration["JWT:Secret"] ?? throw new ArgumentNullException("JWT:Secret is required");
        
        var lifetimeStr = configuration["JWT:Lifetime"] ?? throw new ArgumentNullException("JWT:Lifetime is required");
        Lifetime = TimeSpan.Parse(lifetimeStr);
    }
    
    public SymmetricSecurityKey SymmetricSecurityKey => new(Encoding.ASCII.GetBytes(Secret));
}