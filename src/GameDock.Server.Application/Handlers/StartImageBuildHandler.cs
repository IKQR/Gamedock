using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Fleet;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GameDock.Server.Application.Handlers;

public class StartImageBuildHandler : IRequestHandler<StartImageBuildRequest>
{
    private readonly ILogger _logger;
    private readonly IScheduler _scheduler;

    public StartImageBuildHandler(ILogger<StartImageBuildHandler> logger, IScheduler scheduler)
    {
        _logger = logger;
        _scheduler = scheduler;
    }

    public Task Handle(StartImageBuildRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Preparing for game fleet '{Id}' image build", request.FleetId);

        var jobData = new JobDataMap { ["fleetId"] = request.FleetId };
        var jobDetail = JobBuilder.Create<BackgroundImageBuilderJob>()
            .WithIdentity($"build-{request.FleetId}")
            .UsingJobData(jobData).Build();

        _scheduler.ScheduleJob(jobDetail, TriggerBuilder.Create().StartNow().Build(), cancellationToken);

        _logger.LogInformation("Fleet build '{Id}' scheduled", request.FleetId);

        return Task.CompletedTask;
    }
}

public record StartImageBuildRequest(Guid FleetId) : IRequest;