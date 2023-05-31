using GameDock.Server.Domain.Fleet;
using GameDock.Server.Domain.Session;

namespace GameDock.Server.Application.Services;

public interface ISessionRunner
{
    Task<SessionInfo> RunSessionOnFleetAsync(FleetInfo fleet, CancellationToken cancellationToken = default);
}