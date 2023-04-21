using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameDock.Infrastructure;

public static class ServiceRegister
{
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}