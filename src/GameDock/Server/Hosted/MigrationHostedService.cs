using System.Threading;
using System.Threading.Tasks;
using GameDock.Server.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GameDock.Server.Hosted;

public class MigrationHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MigrationHostedService(IServiceScopeFactory  scopeFactory) => _scopeFactory = scopeFactory;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InfoDbContext>();
        await context.Database.MigrateAsync(cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

public static class MigrationRegistration
{
    public static IServiceCollection MigrateOnStartup(this IServiceCollection services) =>
        services.AddHostedService<MigrationHostedService>();
}