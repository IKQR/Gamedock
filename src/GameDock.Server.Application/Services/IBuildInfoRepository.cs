using GameDock.Server.Domain.Build;
using GameDock.Server.Domain.Enums;

namespace GameDock.Server.Application.Services;

public interface IBuildInfoRepository
{
    Task<BuildInfo> AddAsync(string name, string version, string runtimePath, CancellationToken cancellationToken = default);

    Task<IList<BuildInfo>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<BuildInfo> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IList<BuildInfo>> GetByNameAsync(string names, string versions = null,
        CancellationToken cancellationToken = default);

    Task SetAsDeleted(Guid id, CancellationToken cancellationToken = default);
}