using System.Collections;
using GameDock.Server.Domain;

namespace GameDock.Server.Application.Services;

public interface ISessionInfoRepository
{
    Task SetAsync(SessionInfo session, CancellationToken cancellationToken = default);
    Task SetManyAsync(IEnumerable<SessionInfo> session, CancellationToken cancellationToken = default);
    Task<SessionInfo> GetByIdAsync(string containerId, CancellationToken cancellationToken = default);
}