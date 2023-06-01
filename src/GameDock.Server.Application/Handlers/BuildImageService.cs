using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Build;
using GameDock.Server.Domain.Enums;
using GameDock.Server.Domain.Fleet;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Application.Handlers;

public interface IBuildFleetService
{
    Task<bool> Build(Guid fleetId, CancellationToken cancellationToken = default);
}

public class BuildFleetService : IBuildFleetService
{
    private readonly ILogger<BuildFleetService> _logger;
    private readonly IBuildInfoRepository _buildInfos;
    private readonly IBuildFileRepository _buildFiles;
    private readonly IFleetInfoRepository _fleetInfos;
    private readonly IImageBuilder _imageBuilder;

    public BuildFleetService(ILogger<BuildFleetService> logger, IBuildInfoRepository buildInfos,
        IBuildFileRepository buildFiles, IFleetInfoRepository fleetInfos, IImageBuilder imageBuilder)
    {
        _logger = logger;
        _buildInfos = buildInfos;
        _buildFiles = buildFiles;
        _fleetInfos = fleetInfos;
        _imageBuilder = imageBuilder;
    }

    public async Task<bool> Build(Guid fleetId, CancellationToken cancellationToken = default)
    {
        try
        {
            var info = await PrepareFleetAsync(fleetId, cancellationToken);

            if (info is null) return false;

            var fleet = info.Value.Item1;
            var build = info.Value.Item2;
            await using var source = info.Value.Item3;

            await _imageBuilder.BuildImageFromFleet(fleetId.ToString(), source, fleet.Ports, fleet.Runtime, build.RuntimePath,
                fleet.LaunchParameters, fleet.Variables, cancellationToken);

            await _fleetInfos.SetStatusIfExistAsync(fleetId, FleetStatus.Ready, CancellationToken.None);

            return true;
        }
        catch (Exception)
        {
            await _fleetInfos.SetStatusIfExistAsync(fleetId, FleetStatus.Failed, CancellationToken.None);
            throw;
        }
    }

    private async Task<(FleetInfo, BuildInfo, Stream)?> PrepareFleetAsync(Guid fleetId,
        CancellationToken cancellationToken)
    {
        var fleetInfo = await _fleetInfos.SetStatusIfExistAsync(fleetId, FleetStatus.Pending, cancellationToken);

        if (fleetInfo is null)
        {
            _logger.LogWarning("Unknown fleet image build was requested. Fleet {FleetId}", fleetId);
            return null;
        }

        var buildInfo = await _buildInfos.GetByIdAsync(fleetInfo.BuildId, cancellationToken);

        if (buildInfo is null || buildInfo.Status is BuildStatus.Deleted)
        {
            await _fleetInfos.SetStatusIfExistAsync(fleetId, FleetStatus.Dead, cancellationToken);
            return null;
        }

        var buildFile = await _buildFiles.GetStreamReadAsync(buildInfo.Id.ToString(), cancellationToken);

        if (buildFile is null)
        {
            await _buildInfos.TrySetDeleted(buildInfo.Id, cancellationToken);
            await _fleetInfos.SetStatusIfExistAsync(fleetId, FleetStatus.Dead, cancellationToken);
            return null;
        }

        return (fleetInfo, buildInfo, buildFile);
    }
}