using GameDock.Server.Domain;

namespace GameDock.Server.Application.Services;

public interface ISessionRunner
{
    Task<SessionInfo> RunSessionOnFleetAsync(FleetInfo fleet, CancellationToken cancellationToken = default);
}