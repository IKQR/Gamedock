namespace GameDock.Application.Services;

public interface IBuildFileRepository
{
    Task<Stream> Get(string key, CancellationToken cancellationToken = default);

    Task<string> Add(Stream file, CancellationToken cancellationToken = default);
    
    Task<bool> TryRemove(string key, CancellationToken cancellationToken = default);
}