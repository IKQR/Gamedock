using Docker.DotNet;
using Microsoft.Extensions.DependencyInjection;

namespace GameDock.Server.Utils;

public static class Docker
{
    public static IServiceCollection AddDocker(this IServiceCollection services) =>
        services.AddSingleton<IDockerClient>(_ => new DockerClientConfiguration().CreateClient());
}