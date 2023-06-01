namespace GameDock.Server.Domain;

public record SessionInfo(string ContainerId, Guid FleetId, int[] Ports, DateTime StartedAt, DateTime? StoppedAt = null);