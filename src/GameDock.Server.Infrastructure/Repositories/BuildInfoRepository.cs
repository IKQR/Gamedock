using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Build;
using GameDock.Server.Infrastructure.Database;
using GameDock.Server.Infrastructure.Entities;
using GameDock.Server.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace GameDock.Server.Infrastructure.Repositories;

public class BuildInfoRepository : IBuildInfoRepository
{
    private readonly InfoDbContext _context;

    public BuildInfoRepository(InfoDbContext context)
    {
        _context = context;
    }

    public async Task<BuildInfo> AddAsync(string name, string version, CancellationToken cancellationToken)
    {
        var entity = new BuildInfoEntity
        {
            Name = name,
            Version = version,
        };

        var entry = await _context.BuildInfos.AddAsync(entity, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return BuildInfoMapper.Map(entry.Entity);
    }

    public async Task<BuildInfo> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.BuildInfos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return BuildInfoMapper.Map(entity);
    }

    public async Task<BuildInfo> GetByNameAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        var entity =
            await _context.BuildInfos.FirstOrDefaultAsync(x => x.Name == name && x.Version == version,
                cancellationToken);

        return BuildInfoMapper.Map(entity);
    }
}