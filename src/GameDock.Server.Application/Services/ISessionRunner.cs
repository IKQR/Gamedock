using GameDock.Server.Domain.Session;

namespace GameDock.Server.Application.Services;

public interface ISessionRunner
{
    Task<SessionInfo> RunSessionFromBuild(Guid buildId, CancellationToken cancellationToken = default);
}