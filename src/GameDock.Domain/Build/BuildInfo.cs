using GameDock.Domain.Enums;

namespace GameDock.Domain.Build;

public record BuildInfo(string Id, string Name, string Version, BuildStatus Status);