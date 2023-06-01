using GameDock.Server.Domain.Fleet;
using GameDock.Shared.Dto;

namespace GameDock.Server.Mappers;

public static class FleetInfoMapper
{
    public static FleetInfoDto Map(FleetInfo domain) => new()
    {
        Id = domain.Id,
        BuildId = domain.BuildId,
        Runtime = domain.Runtime,
        Ports = domain.Ports,
        LaunchParameters = domain.LaunchParameters,
        Variables = domain.Variables,
        Status = domain.Status.ToString(),
        CreatedAt = domain.CreatedAt,
        UpdatedAt = domain.UpdatedAt,
    };
}