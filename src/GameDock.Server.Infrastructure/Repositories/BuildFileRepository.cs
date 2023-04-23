using System.Security.AccessControl;
using GameDock.Server.Application.Services;
using Microsoft.Extensions.Options;

namespace GameDock.Server.Infrastructure.Repositories;

public class BuildFileRepository : IBuildFileRepository
{
    private readonly DirectoryInfo _directory;

    public BuildFileRepository(IOptions<FileStorageOptions> options)
    {
        _directory = new DirectoryInfo(options.Value.Path);
    }

    public Stream GetStream(string key, CancellationToken cancellationToken = default)
    {
        var fileName = FileName(key);
        var file = _directory.GetFiles().FirstOrDefault(x => x.Name == fileName);

        return file?.OpenRead();
    }

    public async Task SaveAsync(string key, Stream content, CancellationToken cancellationToken = default)
    {
        var fileName = FileName(key);
        var filePath = Path.Combine(_directory.FullName, fileName);
        var fileInfo = new FileInfo(filePath);

        if (fileInfo.Exists)
        {
            throw new Exception("Build archive already exists");
        }

        await using var fileStream = fileInfo.Create();
        await content.CopyToAsync(fileStream, cancellationToken);
    }

    private static string FileName(string key) => $"{key}.tar";
}

public record FileStorageOptions
{
    public string Path { get; set; }
}