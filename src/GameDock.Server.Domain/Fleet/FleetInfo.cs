using GameDock.Server.Domain.Enums;

namespace GameDock.Server.Domain.Fleet;

public record FleetInfo(Guid Id, Guid BuildId, string Runtime, int[] Ports, string LaunchParameters,
    IDictionary<string, string> Variables,  FleetStatus Status, DateTime CreatedAt, DateTime UpdatedAt);