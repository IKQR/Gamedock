using GameDock.Server.Domain.Build;
using GameDock.Shared.Dto;

namespace GameDock.Server.Mappers;

public static class BuildInfoMapper
{
    public static BuildInfoDto Map(BuildInfo buildInfo) => new()
    {
        Id = buildInfo.Id.ToString(),
        Name = buildInfo.Name,
        Version = buildInfo.Version,
        Status = buildInfo.Status.ToString(),
        CreatedAt = buildInfo.CreatedAt,
        UpdatedAt = buildInfo.UpdatedAt,
    };
}