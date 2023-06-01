using Docker.DotNet;
using Docker.DotNet.Models;
using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Fleet;
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
    
    public async Task<SessionInfo> RunSessionOnFleetAsync(FleetInfo fleet, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Start session requested for fleet {FleetId}", fleet.Id);

        var created = await CreateNew(fleet.Id.ToString(), fleet.Ports, cancellationToken);

        await _docker.Containers.StartContainerAsync(created.ID, new ContainerStartParameters(), cancellationToken);

        var running = await GetRunning(created.ID, cancellationToken);
        var runningPorts = running.Ports.Select(x => (int)x.PublicPort).ToArray();

        return new SessionInfo(created.ID, fleet.Id, runningPorts, running.Created, null);
    }

    private Task<CreateContainerResponse> CreateNew(string imageId, int[] ports, CancellationToken cancellationToken) =>
        _docker.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Name = $"game-server-{Guid.NewGuid()}",
            Image = imageId,
            HostConfig = new HostConfig
            {
                PortBindings = ports.ToDictionary(x => $"{x}/tcp",
                    _ => (IList<PortBinding>)new List<PortBinding> { new() { HostPort = "0" } }),
            },
        }, cancellationToken);

    private async Task<ContainerListResponse> GetRunning(string containerId, CancellationToken cancellationToken) =>
        (await _docker.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true,
            Filters = new Dictionary<string, IDictionary<string, bool>>
            {
                ["id"] = new Dictionary<string, bool>
                {
                    [containerId] = true,
                },
            },
        }, cancellationToken)).SingleOrDefault();
}