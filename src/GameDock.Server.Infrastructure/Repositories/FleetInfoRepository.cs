using GameDock.Server.Application.Services;
using GameDock.Server.Domain;
using GameDock.Server.Domain.Enums;
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
        IDictionary<string, string> variables, CancellationToken cancellationToken)
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

    public async Task<FleetInfo> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = _context.FleetInfos.Where(x => x.Id == id);

        var entity = await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return entity is null ? null : FleetInfoMapper.Map(entity);
    }

    public async Task<FleetInfo> SetImageAsync(Guid id, string image, CancellationToken cancellationToken = default)
    {
        var entity = await _context.FleetInfos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;

        entity.ImageId = image;
        entity.Status = FleetStatus.Ready;
        _context.FleetInfos.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return FleetInfoMapper.Map(entity);
    }

    public async Task<FleetInfo> SetStatusIfExistAsync(Guid id, FleetStatus status, CancellationToken cancellationToken)
    {
        var entity = await _context.FleetInfos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;

        entity.Status = status;
        _context.FleetInfos.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return FleetInfoMapper.Map(entity);
    }

    public async Task<IList<FleetInfo>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.FleetInfos.ToListAsync(cancellationToken: cancellationToken);

        return entities.Select(FleetInfoMapper.Map).ToList();
    }
}