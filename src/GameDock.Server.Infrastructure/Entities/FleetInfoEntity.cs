using GameDock.Server.Domain.Enums;

namespace GameDock.Server.Infrastructure.Entities;

public class FleetInfoEntity
{
    public Guid Id { get; init; }
    public Guid BuildId { get; set; }
    public string ImageId { get; set; }
    public string Runtime { get; init; }
    public int[] Ports { get; set; }
    public string LaunchParameters { get; init; }
    public IDictionary<string, string> Variables { get; set; }
    public FleetStatus Status { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}