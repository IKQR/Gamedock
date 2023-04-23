namespace GameDock.Server.Application.Services;

public interface IBuildFileRepository
{
    Stream GetStream(string key, CancellationToken cancellationToken = default);

    Task SaveAsync(string key, Stream content, CancellationToken cancellationToken = default);
}