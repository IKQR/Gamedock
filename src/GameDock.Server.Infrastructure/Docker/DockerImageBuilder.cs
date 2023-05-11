using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Enums;
using Microsoft.Extensions.Logging;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using SharpCompress.Writers;
using SharpCompress.Writers.Tar;

namespace GameDock.Server.Infrastructure.Docker;

public class DockerImageBuilder : IImageBuilder
{
    private const string Dockerfile = "Dockerfile";

    private readonly ILogger _logger;
    private readonly IDockerClient _docker;
    private readonly IBuildFileRepository _files;
    private readonly IBuildInfoRepository _info;

    public DockerImageBuilder(ILogger<DockerImageBuilder> logger, IDockerClient docker, IBuildFileRepository files,
        IBuildInfoRepository info)
    {
        _logger = logger;
        _docker = docker;
        _files = files;
        _info = info;
    }

    public async Task BuildImageFromArchive(Guid id, string runtimePath, string launchParameters,
        CancellationToken token)
    {
        await _info.ChangeStatus(id, BuildStatus.Building, cancellationToken: token);
        try
        {
            await _docker.System.PingAsync(token);

            var dockerSystem = await _docker.System.GetSystemInfoAsync(token);

            var isWindows =
                dockerSystem.OperatingSystem.StartsWith("Windows", StringComparison.InvariantCultureIgnoreCase);

            var dockerfileContent = isWindows ? "" : Dockerfiles.GetLinuxDockerfile(runtimePath, launchParameters);

            var original = _files.GetStream(id.ToString(), token);
            var buildArchive = await AddDockerfileIfNotExists(original, dockerfileContent, true);

            var buildParameters = new ImageBuildParameters
            {
                Dockerfile = Dockerfile,
                Tags = new List<string> { $"game-server-{(isWindows ? "win" : "unix")}:latest" },
            };

            var buildProgress = new Progress<JSONMessage>((x) => HandleBuildProgress(id, x));

            await _docker.Images.BuildImageFromDockerfileAsync(
                buildParameters,
                buildArchive,
                null,
                null,
                buildProgress,
                CancellationToken.None);

            await _info.ChangeStatus(id, BuildStatus.Ready, cancellationToken: token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while building new server {Id}", id);
            await _info.ChangeStatus(id, BuildStatus.Failed, CancellationToken.None);
            throw;
        }
    }

    private void HandleBuildProgress(Guid buildId, JSONMessage message)
    {
        _logger.LogInformation("Process {Id}: {@Message}", buildId, message);
    }

    private static async ValueTask<Stream> AddDockerfileIfNotExists(Stream input, string dockerfile,
        bool disposeInput = false)
    {
        using var inputArchive = TarArchive.Open(input);

        var resultStream = new MemoryStream();

        if (inputArchive.Entries.Any(x => x.Key.EndsWith("Dockerfile")))
        {
            await input.CopyToAsync(resultStream);
        }
        else
        {
            using var tarWriter = WriterFactory.Open(resultStream, ArchiveType.Tar,
                new TarWriterOptions(CompressionType.None, true));

            foreach (var entry in inputArchive.Entries)
            {
                await using var entryStream = entry.OpenEntryStream();
                using var tempEntryStream = new MemoryStream();
                await entryStream.CopyToAsync(tempEntryStream);
                tempEntryStream.Position = 0;
                tarWriter.Write(entry.Key, tempEntryStream);
            }

            using var dockerfileStream = new MemoryStream(Encoding.UTF8.GetBytes(dockerfile));
            tarWriter.Write(Dockerfile, dockerfileStream);
        }

        if (disposeInput)
        {
            await input.DisposeAsync();
        }

        resultStream.Position = 0;

        return resultStream;
    }
}

public static class Dockerfiles
{
    public static string GetLinuxDockerfile(string runtimePath, string launchParameters) => $@"
            FROM ubuntu:20.04
            EXPOSE 5000
            WORKDIR /app
            COPY . .
            ENTRYPOINT [""./{runtimePath}"", ""{launchParameters}""]
        ";
}