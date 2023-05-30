using GameDock.Server.Domain.Build;
using GameDock.Server.Domain.Fleet;
using GameDock.Server.Infrastructure.Entities;

namespace GameDock.Server.Infrastructure.Mappers;

public static class FleetInfoMapper
{
    public static FleetInfo Map(FleetInfoEntity entity) =>
        new(
            Id: entity.Id,
            BuildId: entity.BuildId,
            Runtime: entity.Runtime,
            Ports: entity.Ports,
            LaunchParameters: entity.LaunchParameters,
            Variables: entity.Variables,
            Status: entity.Status,
            ImageId: entity.ImageId,
            CreatedAt: entity.CreatedAt,
            UpdatedAt: entity.UpdatedAt);
}