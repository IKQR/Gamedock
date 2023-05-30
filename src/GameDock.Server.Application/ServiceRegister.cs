using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GameDock.Server.Application;

public static class ServiceRegister
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
        => services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
}