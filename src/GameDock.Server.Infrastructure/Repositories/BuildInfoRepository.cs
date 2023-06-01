using GameDock.Server.Application.Services;
using GameDock.Server.Domain;
using GameDock.Server.Domain.Enums;
using GameDock.Server.Infrastructure.Database;
using GameDock.Server.Infrastructure.Entities;
using GameDock.Server.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace GameDock.Server.Infrastructure.Repositories;

public class BuildInfoRepository : IBuildInfoRepository
{
    private readonly InfoDbContext _context;

    public BuildInfoRepository(InfoDbContext context) => _context = context;

    public async Task<BuildInfo> AddAsync(string name, string version, string runtimePath,
        CancellationToken cancellationToken)
    {
        var entity = new BuildInfoEntity
        {
            Name = name,
            Version = version,
            RuntimePath = runtimePath,
        };

        var entry = await _context.BuildInfos.AddAsync(entity, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return BuildInfoMapper.Map(entry.Entity);
    }

    public async Task<IList<BuildInfo>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.BuildInfos
            .Where(x => x.Status != BuildStatus.Deleted)
            .ToListAsync(cancellationToken: cancellationToken);

        return entities.Select(BuildInfoMapper.Map).ToList();
    }

    public async Task<BuildInfo> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.BuildInfos.Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return BuildInfoMapper.Map(entity);
    }

    public async Task<IList<BuildInfo>> GetByNameAsync(string name, string version, CancellationToken cancellationToken)
    {
        var query = _context.BuildInfos.Where(x => x.Name == name);

        if (version is not null)
        {
            query = query.Where(x => x.Version == version);
        }

        var result = await query.ToListAsync(cancellationToken: cancellationToken);

        return result.Select(BuildInfoMapper.Map).ToList();
    }

    public async Task<bool> TrySetDeleted(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _context.BuildInfos.FindAsync(new object[] { id }, cancellationToken);

            if (entity is null)
            {
                return false;
            }

            entity.Status = BuildStatus.Deleted;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}