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
        IConfiguration configuration)
        => services
            .AddDbContext<InfoDbContext>(opt => opt.UseSqlite(configuration.GetConnectionString("SQLite")))
            .AddScoped<ITransactionManager, TransactionManager>()
            .AddScoped<IBuildInfoRepository, BuildInfoRepository>()
            .AddScoped<IBuildFileRepository, BuildFileRepository>()
            .Configure<FileStorageOptions>(opt => opt.Path = configuration.GetConnectionString("FileStorage"))
            .AddScoped<IImageBuilder, DockerImageBuilder>()
            .AddSingleton<IDockerClient>(_ => new DockerClientConfiguration().CreateClient());
}