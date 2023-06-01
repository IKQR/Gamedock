using GameDock.Server.Application.Services;
using GameDock.Server.Domain;

namespace GameDock.Server.Infrastructure.Repositories;

public class SessionInfoRepository : ISessionInfoRepository
{
    public Task SetAsync(SessionInfo session, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SetManyAsync(IEnumerable<SessionInfo> session, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SessionInfo> GetByIdAsync(string containerId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}