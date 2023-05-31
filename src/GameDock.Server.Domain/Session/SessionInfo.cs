namespace GameDock.Server.Domain.Session;

public record SessionInfo(string ContainerId, Guid FleetId, int[] Ports, DateTime StartedAt, DateTime? StoppedAt = null);