namespace GameDock.Application.Services;

public interface IBuildFileRepository
{
    Task<Stream> Get(string key, CancellationToken cancellationToken = default);

    Task<string> Add(string key, Stream file, CancellationToken cancellationToken = default);
}