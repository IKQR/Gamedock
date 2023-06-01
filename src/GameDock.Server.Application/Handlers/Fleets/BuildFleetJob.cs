using Microsoft.Extensions.Logging;
using Quartz;

namespace GameDock.Server.Application.Handlers.Fleets;

internal class BuildFleetJob : IJob
{
    private readonly ILogger _logger;
    private readonly IBuildFleetService _buildFleetService;

    public BuildFleetJob(ILogger<BuildFleetJob> logger, IBuildFleetService buildBuildFleetService)
    {
        _logger = logger;
        _buildFleetService = buildBuildFleetService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context.CancellationToken;
        var fleetId = context.JobDetail.JobDataMap.GetGuidValue("fleetId");
        
        _logger.LogInformation("Build image process started. FleetId: '{Id}'", fleetId);

        try
        {
            await _buildFleetService.Build(fleetId, cancellationToken);

            _logger.LogInformation("Fleet image ready. FleetId: '{Id}'", fleetId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while building image");
            throw;
        }
    }
}