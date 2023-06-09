﻿using GameDock.Server.Domain;
using GameDock.Server.Infrastructure.Entities;

namespace GameDock.Server.Infrastructure.Mappers;

public static class FleetInfoMapper
{
    public static FleetInfo Map(FleetInfoEntity entity) =>
        new(
            Id: entity.Id,
            BuildId: entity.BuildId,
            ImageKey: entity.ImageId,
            Runtime: entity.Runtime,
            Ports: entity.Ports,
            LaunchParameters: entity.LaunchParameters,
            Variables: entity.Variables,
            Status: entity.Status,
            CreatedAt: entity.CreatedAt,
            UpdatedAt: entity.UpdatedAt);
}