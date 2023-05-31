using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Enums;
using GameDock.Server.Domain.Fleet;
using GameDock.Server.Infrastructure.Database;
using GameDock.Server.Infrastructure.Entities;
using GameDock.Server.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

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

    public async Task<FleetInfo> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = _context.FleetInfos.Where(x => x.Id == id);

        var entity = await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return entity is null ? null : FleetInfoMapper.Map(entity);
    }
}