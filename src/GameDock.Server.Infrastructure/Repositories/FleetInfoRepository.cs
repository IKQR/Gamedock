using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Enums;
using GameDock.Server.Domain.Fleet;
using GameDock.Server.Infrastructure.Database;
using GameDock.Server.Infrastructure.Entities;
using GameDock.Server.Infrastructure.Mappers;

namespace GameDock.Server.Infrastructure.Repositories;

public class FleetInfoRepository : IFleetInfoRepository
{
    private readonly InfoDbContext _context;

    public FleetInfoRepository(InfoDbContext context)
    {
        _context = context;
    }

    public async Task<FleetInfo> AddAsync(Guid buildId, string runtime, int[] ports, string launchParameters,
        IDictionary<string, string> variables, CancellationToken cancellationToken = default)
    {
        var entity = new FleetInfoEntity
        {
            BuildId = buildId,
            Runtime = runtime,
            Variables = variables,
            LaunchParameters = launchParameters,
            Ports = ports,
            Status = FleetStatus.Created,
        };

        var entry = await _context.FleetInfos.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return FleetInfoMapper.Map(entry.Entity);
    }
}