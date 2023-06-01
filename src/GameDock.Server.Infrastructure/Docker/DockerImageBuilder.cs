using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using GameDock.Server.Application.Services;
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

    public DockerImageBuilder(ILogger<DockerImageBuilder> logger, IDockerClient docker)
    {
        _logger = logger;
        _docker = docker;
    }

    public async Task<string> BuildImageFromFleet(string key, Stream sourceCode, int[] ports, string runtime,
        string entrypointFile, string launchParameters, IDictionary<string, string> environmentVariables,
        CancellationToken cancellationToken)
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), key);

        var dockerfile = new DockerConfigurationBuilder
        {
            Ports = ports,
            RuntimeKey = runtime,
            EntrypointFile = entrypointFile,
            LaunchParameters = launchParameters,
            EnvironmentVariables = environmentVariables,
        }.Build();

        try
        {
            await PrepareSourceArchiveAsync(sourceCode, tempFilePath, dockerfile);

            await using var source = File.OpenRead(tempFilePath);

            var imageName = $"game-server-{key}:latest";

            var buildParameters = new ImageBuildParameters
            {
                Dockerfile = Dockerfile,
                Tags = new List<string> { imageName },
            };

            var buildProgress = new Progress<JSONMessage>(x => HandleBuildProgress(key, x));

            await _docker.Images.BuildImageFromDockerfileAsync(
                buildParameters,
                source,
                null,
                null,
                buildProgress,
                cancellationToken);

            var images = await _docker.Images.ListImagesAsync(new ImagesListParameters
            {
                All = true,
                Filters = new Dictionary<string, IDictionary<string, bool>>()
                {
                    ["reference"] = new Dictionary<string, bool>()
                    {
                        [imageName] = true,
                    },
                },
            }, CancellationToken.None);

            return images.Single().ID;
        }
        finally
        {
            if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
        }
    }

    private void HandleBuildProgress(string key, JSONMessage message)
    {
        _logger.LogInformation("Process {Id}: {@Message}", key, message);
    }

    private async ValueTask PrepareSourceArchiveAsync(Stream original, string tempFilePath, string dockerfile)
    {
        using var inputArchive = TarArchive.Open(original);

        await using var resultStream = File.Create(tempFilePath);

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
}