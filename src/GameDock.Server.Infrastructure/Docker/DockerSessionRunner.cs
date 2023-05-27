using Docker.DotNet;
using Docker.DotNet.Models;
using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Session;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Infrastructure.Docker;

public class DockerSessionRunner : ISessionRunner
{
    private readonly ILogger _logger;
    private readonly IDockerClient _docker;

    public DockerSessionRunner(ILogger<DockerSessionRunner> logger, IDockerClient docker)
    {
        _logger = logger;
        _docker = docker;
    }

    public async Task<SessionInfo> RunSessionFromBuild(Guid buildId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Start session requested for build {BuildId}", buildId);

        var running = await GetRunning(buildId, cancellationToken);

        var current = running.FirstOrDefault(x => x.Ports.Any(p => p.PublicPort == 5000));
        if (current is not null)
        {
            return new SessionInfo(current.ID, buildId, 5000, current.Created);
        }

        var created = await CreateNew(buildId, cancellationToken);

        await _docker.Containers.StartContainerAsync(created.ID, new ContainerStartParameters(), cancellationToken);

        return new SessionInfo(created.ID, buildId, 5000, DateTime.UtcNow);
    }

    private Task<IList<ContainerListResponse>> GetRunning(Guid buildId, CancellationToken cancellationToken) =>
        _docker.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true,
            Filters = new Dictionary<string, IDictionary<string, bool>>
            {
                ["ancestor"] = new Dictionary<string, bool>
                {
                    [$"game-server-{buildId}:latest"] = true,
                },
                ["status"] = new Dictionary<string, bool>
                {
                    ["running"] = true,
                },
            },
        }, cancellationToken);

    private Task<CreateContainerResponse> CreateNew(Guid buildId, CancellationToken cancellationToken) =>
        _docker.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Name = "game-server",
            Image = $"game-server-{buildId}:latest",
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    ["5000/tcp"] = new List<PortBinding>
                    {
                        new() { HostPort = "5000" },
                    },
                },
            },
        }, cancellationToken);
}