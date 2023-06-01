namespace GameDock.Server.Application.Services;

public interface IBuildFileRepository
{
    ValueTask<Stream> GetStreamReadAsync(string key, CancellationToken cancellationToken = default);

    Task SaveAsync(string key, Stream content, CancellationToken cancellationToken = default);
    ValueTask<bool> TryDeleteAsync(string key, CancellationToken cancellationToken);
}