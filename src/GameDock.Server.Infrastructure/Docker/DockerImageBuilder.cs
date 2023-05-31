using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Enums;
using GameDock.Server.Infrastructure.Database;
using GameDock.Server.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
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
    private readonly InfoDbContext _infoContext;
    private readonly IBuildFileRepository _files;

    public DockerImageBuilder(ILogger<DockerImageBuilder> logger, IDockerClient docker, InfoDbContext infoContext,
        IBuildFileRepository files)
    {
        _files = files;
        _logger = logger;
        _docker = docker;
        _infoContext = infoContext;
    }

    public async Task BuildImageFromFleet(Guid fleetId, CancellationToken cancellationToken = default)
    {
        var (fleetInfo, buildInfo) = await GetLockedFleet(fleetId, cancellationToken);

        try
        {
            var originalFile = buildInfo.Id.ToString();
            var tempFile = Path.Combine(Path.GetTempPath(), fleetId.ToString());
            var dockerfile = DockerfileFactory
                .GetLinuxDockerfile(fleetInfo.Ports, buildInfo.RuntimePath, fleetInfo.LaunchParameters);

            await PrepareSourceFile(originalFile, tempFile, dockerfile);
            await using var source = File.OpenRead(tempFile);

            var imageName = $"game-server-{fleetId}:latest";

            var buildParameters = new ImageBuildParameters
            {
                Dockerfile = Dockerfile,
                Tags = new List<string> { imageName },
            };

            var buildProgress = new Progress<JSONMessage>((x) => HandleBuildProgress(fleetId, x));

            await _docker.Images.BuildImageFromDockerfileAsync(
                buildParameters,
                source,
                null,
                null,
                buildProgress,
                cancellationToken);

            fleetInfo.Status = FleetStatus.Ready;
            fleetInfo.ImageId = imageName;
        }
        finally
        {
            await UnlockFleet(fleetInfo, CancellationToken.None);
        }
    }

    private void HandleBuildProgress(Guid fleetId, JSONMessage message)
    {
        _logger.LogInformation("Process {Id}: {@Message}", fleetId, message);
    }

    private async ValueTask PrepareSourceFile(string originalPath, string tempFilePath,
        string dockerfile)
    {
        await using var originalFile = await _files.GetStream(originalPath);

        using var inputArchive = TarArchive.Open(originalFile);

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

    private async Task<(FleetInfoEntity, BuildInfoEntity)> GetLockedFleet(Guid fleetId,
        CancellationToken cancellationToken)
    {
        var fleetInfo = _infoContext.FleetInfos.FirstOrDefault(f => f.Id == fleetId);

        if (fleetInfo is null)
        {
            throw new KeyNotFoundException($"Fleet '{fleetId}' not found.");
        }

        if (fleetInfo.Status is FleetStatus.Pending)
        {
            throw new InvalidOperationException($"Fleet '{fleetId}' is in Pending sate");
        }

        fleetInfo.Status = FleetStatus.Pending;
        _infoContext.Update(fleetInfo);
        await _infoContext.SaveChangesAsync(cancellationToken);

        var buildInfo = await _infoContext.BuildInfos.FirstOrDefaultAsync(b => b.Id == fleetInfo.BuildId,
            cancellationToken: cancellationToken);

        if (buildInfo == null || buildInfo.Status is BuildStatus.Deleted)
        {
            throw new InvalidOperationException(
                $"Fleet '{fleetId}' can't be build. Build '{fleetInfo.BuildId}' deleted");
        }

        return (fleetInfo, buildInfo);
    }

    private async Task UnlockFleet(FleetInfoEntity fleetInfo, CancellationToken cancellationToken)
    {
        if (fleetInfo.Status is FleetStatus.Pending)
        {
            fleetInfo.Status = FleetStatus.Failed;
        }

        _infoContext.Update(fleetInfo);

        await _infoContext.SaveChangesAsync(cancellationToken);
    }
}

public static class DockerfileFactory
{
    public static string GetLinuxDockerfile(IEnumerable<int> ports, string runtimePath, string launchParameters) => $@"
            FROM mcr.microsoft.com/dotnet/runtime:7.0
            {string.Join(Environment.NewLine, ports.Select(x => $"EXPOSE " + x))}
            WORKDIR /app
            COPY . .
            ENTRYPOINT [""./{runtimePath}"", ""{launchParameters}""]
        ";
}