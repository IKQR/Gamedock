using Docker.DotNet;
using Docker.DotNet.Models;
using GameDock.Server.Application.Services;
using GameDock.Server.Domain;
using GameDock.Server.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Infrastructure.Docker;

public class DockerSessionManager : ISessionManager
{
    private readonly ILogger _logger;
    private readonly IDockerClient _docker;

    public DockerSessionManager(ILogger<DockerSessionManager> logger, IDockerClient docker)
    {
        _logger = logger;
        _docker = docker;
    }

    public async Task<SessionInfo> RunAsync(string imageKey, int[] exposedPorts, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start session requested for image {FleetId}", imageKey);

        var created = await CreateAsync(imageKey, exposedPorts, cancellationToken);

        await _docker.Containers.StartContainerAsync(created.ID, new ContainerStartParameters(), cancellationToken);

        var running = await GetByIdAsync(created.ID, cancellationToken);
        var runningPorts = running.Ports.Select(x => (int)x.PublicPort).ToArray();

        return new SessionInfo(created.ID, imageKey, runningPorts, ParseStatus(running.Status), running.Created,
            DateTime.UtcNow);
    }

    public async Task<bool> StopAsync(string containerId, CancellationToken cancellationToken)
    {
        var container = await GetByIdAsync(containerId, cancellationToken);

        if (container is null)
        {
            return false;
        }

        if (container.Status != "running")
        {
            return false;
        }

        var stopResult = await _docker.Containers.StopContainerAsync(container.ID, new(), cancellationToken);

        if (!stopResult)
        {
            return false;
        }

        await _docker.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters(), cancellationToken);

        return true;
    }

    public async Task<SessionInfo> GetStatusAsync(string containerId, CancellationToken cancellationToken)
    {
        var running = await GetByIdAsync(containerId, cancellationToken);

        if (running is null)
        {
            return null;
        }
        
        var runningPorts = running.Ports.Select(x => (int)x.PublicPort).ToArray();

        return new SessionInfo(running.ID, running.Image, runningPorts, ParseStatus(running.Status), running.Created,
            null);
    }

    public Task<IList<SessionInfo>> GetGroupStatusAsync(string imageKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private Task<CreateContainerResponse> CreateAsync(string imageId, int[] ports,
        CancellationToken cancellationToken = default) =>
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

    private async Task<ContainerListResponse> GetByIdAsync(string containerId,
        CancellationToken cancellationToken = default) =>
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
    
    
    private async Task<ContainerListResponse> GetByImageAsync(string imageKey,
        CancellationToken cancellationToken = default) =>
        (await _docker.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true,
            Filters = new Dictionary<string, IDictionary<string, bool>>
            {
                ["id"] = new Dictionary<string, bool>
                {
                    [imageKey] = true,
                },
            },
        }, cancellationToken)).SingleOrDefault();

    private static SessionStatus ParseStatus(string status) => status switch
    {
        "created" => SessionStatus.Created,
        "running" => SessionStatus.Running,
        "exited" => SessionStatus.Exited,
        "dead" => SessionStatus.Dead,
        _ => SessionStatus.Undefined,
    };
}