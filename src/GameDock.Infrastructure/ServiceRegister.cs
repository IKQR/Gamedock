using GameDock.Application.Services;
using GameDock.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameDock.Infrastructure;

public static class ServiceRegister
{
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
        => services
            .AddDbContext<InfoDbContext>(opt => opt.UseSqlite(configuration.GetConnectionString("SQLite")))
            .AddScoped<IBuildInfoRepository, BuildInfoRepository>();
}