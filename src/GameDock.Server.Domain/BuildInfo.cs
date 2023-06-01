using GameDock.Server.Domain.Enums;

namespace GameDock.Server.Domain;

public record BuildInfo(Guid Id, string Name, string Version, string RuntimePath, BuildStatus Status, DateTime CreatedAt);