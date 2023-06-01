using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GameDock.Server.Application.Handlers;

public class ScheduleImageBuildHandler : IRequestHandler<ScheduleImageBuildRequest>
{
    private readonly ILogger _logger;
    private readonly IScheduler _scheduler;

    public ScheduleImageBuildHandler(ILogger<ScheduleImageBuildHandler> logger, IScheduler scheduler)
    {
        _logger = logger;
        _scheduler = scheduler;
    }

    public Task Handle(ScheduleImageBuildRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Preparing for game fleet '{Id}' image build", request.FleetId);

        var jobData = new JobDataMap { ["fleetId"] = request.FleetId };
        var jobDetail = JobBuilder.Create<BuildFleetJob>()
            .WithIdentity($"build-{request.FleetId}")
            .UsingJobData(jobData).Build();

        _scheduler.ScheduleJob(jobDetail, TriggerBuilder.Create().StartNow().Build(), cancellationToken);

        _logger.LogInformation("Fleet build '{Id}' scheduled", request.FleetId);

        return Task.CompletedTask;
    }
}

public record ScheduleImageBuildRequest(Guid FleetId) : IRequest;