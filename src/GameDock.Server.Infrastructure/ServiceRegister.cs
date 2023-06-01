using Docker.DotNet;
using GameDock.Server.Application.Services;
using GameDock.Server.Infrastructure.Database;
using GameDock.Server.Infrastructure.Docker;
using GameDock.Server.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameDock.Server.Infrastructure;

public static class ServiceRegister
{
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services,
        IConfiguration configuration) => services
        .ConfigureDocker(configuration)
        .ConfigureDatabase(configuration);

    private static IServiceCollection ConfigureDocker(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddScoped<IImageBuilder, DockerImageBuilder>()
            .AddSingleton<IDockerClient>(_ => new DockerClientConfiguration().CreateClient())
            .AddScoped<ISessionManager, DockerSessionManager>();


    private static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<FileStorageOptions>(opt => opt.Path = configuration.GetConnectionString("FileStorage"))
            .AddDbContext<InfoDbContext>(opt => opt.UseSqlite(configuration.GetConnectionString("SQLite")))
            .AddScoped<ITransactionManager, TransactionManager>()
            .AddScoped<IBuildInfoRepository, BuildInfoRepository>()
            .AddScoped<IBuildFileRepository, BuildFileRepository>()
            .AddScoped<IFleetInfoRepository, FleetInfoRepository>()
            .AddScoped<ISessionInfoRepository, SessionInfoRepository>();
}