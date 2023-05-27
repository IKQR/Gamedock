namespace GameDock.Server.Domain.Session;

public record SessionInfo(string Id, Guid BuildId, ushort Port, DateTime StartedAt);