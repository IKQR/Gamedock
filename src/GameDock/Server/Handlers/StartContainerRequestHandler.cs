using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Handlers;

public class StartContainerRequestHandler : IRequestHandler<StartContainerRequest>
{
    private readonly ILogger _logger;
    private readonly IDockerClient _docker;

    public StartContainerRequestHandler(ILogger<StartContainerRequestHandler> logger, IDockerClient docker)
    {
        _logger = logger;
        _docker = docker;
    }

    public async Task Handle(StartContainerRequest request, CancellationToken cancellationToken)
    {
        // Create a new container with the specified container name and image
        var containerConfig = new Config
        {
            Image = request.ImageName,
            AttachStdin = false,
            AttachStdout = true,
            AttachStderr = true,
            Tty = true,
            OpenStdin = false,
            StdinOnce = false,
        };

        var containerCreateResponse = await _docker.Containers.CreateContainerAsync(
            new CreateContainerParameters(containerConfig) { Name = request.ContainerName }, cancellationToken);

        // Start the container
        await _docker.Containers.StartContainerAsync(containerCreateResponse.ID, new ContainerStartParameters(),
            cancellationToken);
    }
}

public record StartContainerRequest(string ImageName, string ContainerName) : IRequest;