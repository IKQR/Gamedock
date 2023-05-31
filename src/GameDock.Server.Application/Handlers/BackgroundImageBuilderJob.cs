using GameDock.Server.Application.Services;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GameDock.Server.Application.Handlers;

public class BackgroundImageBuilderJob : IJob
{
    private readonly ILogger _logger;
    private readonly IImageBuilder _imageBuilder;

    public BackgroundImageBuilderJob(ILogger<BackgroundImageBuilderJob> logger, IImageBuilder imageBuilder)
    {
        _logger = logger;
        _imageBuilder = imageBuilder;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context.CancellationToken;
        var fleetId = context.JobDetail.JobDataMap.GetGuidValue("fleetId");
        
        _logger.LogInformation("Build image process started. FleetId: '{Id}'", fleetId);

        try
        {
            await _imageBuilder.BuildImageFromFleet(fleetId, cancellationToken);

            _logger.LogInformation("Fleet image ready. FleetId: '{Id}'", fleetId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while building image");
            throw;
        }
    }
}