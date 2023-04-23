using System.Reflection;
using Docker.DotNet;
using Microsoft.Extensions.DependencyInjection;

namespace GameDock.Application;

public static class ServiceRegister
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
        => services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            .AddSingleton<IDockerClient>(_ => new DockerClientConfiguration().CreateClient());
}