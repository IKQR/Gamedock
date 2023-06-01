using System.Reflection;
using GameDock.Server.Application.Handlers;
using GameDock.Server.Application.Handlers.Fleets;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace GameDock.Server.Application;

public static class ServiceRegister
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
        => services
            .AddScoped<IBuildFleetService, BuildFleetService>()
            .AddQuartz(q => q.UseMicrosoftDependencyInjectionJobFactory())
            .AddQuartzHostedService(q => q.WaitForJobsToComplete = true)
            .AddSingleton(sp => sp.GetRequiredService<ISchedulerFactory>().GetScheduler().Result)
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
}