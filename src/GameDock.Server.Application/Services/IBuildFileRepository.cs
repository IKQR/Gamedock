namespace GameDock.Server.Application.Services;

public interface IBuildFileRepository
{
    ValueTask<Stream> GetStream(string key, CancellationToken cancellationToken = default);

    Task SaveAsync(string key, Stream content, CancellationToken cancellationToken = default);
}