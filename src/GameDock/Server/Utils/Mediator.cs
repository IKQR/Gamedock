using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GameDock.Server.Utils;

public static class Mediator
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
        => services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
}