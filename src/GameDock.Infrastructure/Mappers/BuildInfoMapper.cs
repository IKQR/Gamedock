using GameDock.Domain.Build;
using GameDock.Infrastructure.Entities;

namespace GameDock.Infrastructure.Mappers;

public static class BuildInfoMapper
{
    public static BuildInfo Map(BuildInfoEntity entity)=>
        new(
            Id: entity.Id,
            Name: entity.Name,
            Version: entity.Version,
            Status: entity.Status,
            CreatedAt: entity.CreatedAt,
            UpdatedAt: entity.UpdatedAt);
}