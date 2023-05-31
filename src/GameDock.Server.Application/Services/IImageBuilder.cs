namespace GameDock.Server.Application.Services;

public interface IImageBuilder
{
    Task BuildImageFromFleet(Guid fleetId, CancellationToken cancellationToken = default);
}