using GameDock.Server.Domain.Session;
using GameDock.Shared.Dto;

namespace GameDock.Server.Mappers;

public static class SessionInfoMapper
{
    public static SessionInfoDto Map(SessionInfo buildInfo) => new()
    {
        Id = buildInfo.Id,
        Port = buildInfo.Port,
        BuildId = buildInfo.BuildId,
        StartedAt = buildInfo.StartedAt,
    };
}