using GameDock.Server.Domain.Enums;
using GameDock.Server.Domain.Fleet;

namespace GameDock.Server.Application.Services;

public interface IFleetInfoRepository
{
    Task<FleetInfo> AddAsync(Guid buildId, string runtime, int[] ports, string launchParameters,
        IDictionary<string, string> variables, CancellationToken cancellationToken = default);
    
    Task<FleetInfo> GetById(Guid id, CancellationToken cancellationToken = default);
    
    Task<FleetInfo> SetStatusIfExistAsync(Guid id, FleetStatus status, CancellationToken cancellationToken = default);

}