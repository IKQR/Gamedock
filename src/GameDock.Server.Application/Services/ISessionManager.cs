using GameDock.Server.Domain;

namespace GameDock.Server.Application.Services;

public interface ISessionManager
{
    Task<SessionInfo> RunAsync(string imageKey, int[] exposedPorts, CancellationToken cancellationToken = default);
    Task<bool> StopAsync(string containerId, CancellationToken cancellationToken = default);
    Task<SessionInfo> GetStatusAsync(string containerId, CancellationToken cancellationToken = default);
    Task<IList<SessionInfo>> GetGroupStatusAsync(string imageKey, CancellationToken cancellationToken = default);
}