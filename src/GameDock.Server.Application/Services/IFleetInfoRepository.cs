using GameDock.Server.Domain;
using GameDock.Server.Domain.Enums;

namespace GameDock.Server.Application.Services;

public interface IFleetInfoRepository
{
    Task<FleetInfo> AddAsync(Guid buildId, string runtime, int[] ports, string launchParameters,
        IDictionary<string, string> variables, CancellationToken cancellationToken = default);

    Task<FleetInfo> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<FleetInfo> SetImageAsync(Guid id, string image, CancellationToken cancellationToken = default);
    
    Task<FleetInfo> SetStatusIfExistAsync(Guid id, FleetStatus status, CancellationToken cancellationToken = default);

    Task<IList<FleetInfo>> GetAllAsync(CancellationToken cancellationToken = default);
}