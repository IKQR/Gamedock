using GameDock.Domain.Build;

namespace GameDock.Application.Services;

public interface IBuildInfoRepository
{
    Task Add(BuildInfo info);
    
    Task<BuildInfo> GetById(string id, CancellationToken cancellationToken = default);
    
    Task<BuildInfo> GetByName(string name, string version, CancellationToken cancellationToken = default);
}