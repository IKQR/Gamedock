﻿using GameDock.Domain.Enums;

namespace GameDock.Infrastructure.Entities;

public class BuildInfoEntity
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Version { get; init; }
    public BuildStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}