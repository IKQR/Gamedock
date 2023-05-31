using GameDock.Server.Domain.Session;
using GameDock.Shared.Dto;

namespace GameDock.Server.Mappers;

public static class SessionInfoMapper
{
    public static SessionInfoDto Map(SessionInfo sessionInfo) => new()
    {
        Ip = "176.36.111.97",
        Id = sessionInfo.ContainerId,
        Ports = sessionInfo.Ports,
        BuildId = sessionInfo.FleetId,
        StartedAt = sessionInfo.StartedAt,
    };
}