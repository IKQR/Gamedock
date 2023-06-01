using GameDock.Server.Domain.Enums;

namespace GameDock.Server.Domain;

public record SessionInfo(string ContainerId, string ImageId, int[] Ports, SessionStatus Status, DateTime CreatedAt, DateTime? UpdatedAt = null);