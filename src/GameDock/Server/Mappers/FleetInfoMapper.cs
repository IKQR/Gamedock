using GameDock.Server.Domain.Build;
using GameDock.Server.Domain.Fleet;
using GameDock.Shared.Dto;
using GameDock.Shared.Responses;

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
        ImageId = domain.ImageId,
        CreatedAt = domain.CreatedAt,
        UpdatedAt = domain.UpdatedAt,
    };
}