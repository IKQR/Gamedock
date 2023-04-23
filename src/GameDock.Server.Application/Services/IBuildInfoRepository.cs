using GameDock.Server.Domain.Build;

namespace GameDock.Server.Application.Services;

public interface IBuildInfoRepository
{
    Task<BuildInfo> AddAsync(string name, string version, CancellationToken cancellationToken = default);
    
    Task<BuildInfo> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<BuildInfo> GetByNameAsync(string name, string version, CancellationToken cancellationToken = default);
}