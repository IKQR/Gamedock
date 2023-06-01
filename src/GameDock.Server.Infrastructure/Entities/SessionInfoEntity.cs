using GameDock.Server.Domain.Enums;

namespace GameDock.Server.Infrastructure.Entities;

public class SessionInfoEntity
{
    public string ContainerId { get; set; }
    public string ImageId { get; set; }
    public int[] Ports { get; set; }
    public SessionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}