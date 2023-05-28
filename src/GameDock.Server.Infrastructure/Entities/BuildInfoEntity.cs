using GameDock.Server.Domain.Enums;

namespace GameDock.Server.Infrastructure.Entities;

public class BuildInfoEntity
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Version { get; init; }
    public string RuntimePath { get; set; }
    public BuildStatus Status { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}