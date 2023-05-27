using GameDock.Server.Domain.Build;
using GameDock.Server.Domain.Enums;

namespace GameDock.Server.Application.Services;

public interface IBuildInfoRepository
{
    Task<BuildInfo> AddAsync(string name, string version, CancellationToken cancellationToken = default);

    Task<IList<BuildInfo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IList<BuildInfo>> GetByIdAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task ChangeStatus(Guid id, BuildStatus status, CancellationToken cancellationToken = default);
}