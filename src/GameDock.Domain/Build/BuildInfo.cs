using GameDock.Domain.Enums;

namespace GameDock.Domain.Build;

public record BuildInfo(Guid Id, string Name, string Version, BuildStatus Status, DateTime CreatedAt,
    DateTime UpdatedAt = default);