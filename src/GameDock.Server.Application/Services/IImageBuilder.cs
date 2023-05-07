namespace GameDock.Server.Application.Services;

public interface IImageBuilder
{
    Task BuildImageFromArchive(Guid id, string runtimePath, string launchParameters,
        CancellationToken cancellationToken = default);
}