using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using GameDock.Server.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Handlers;

public class StartBuildRequestHandler : IRequestHandler<StartBuildRequest,StartBuildResponse>
{
    private readonly ILogger _logger;
    private readonly IDockerClient _docker;
    private readonly IHubContext<ImageHub> _imageHub;

    public StartBuildRequestHandler(ILogger<StartBuildRequestHandler> logger, IDockerClient docker,
        IHubContext<ImageHub> imageHub)
    {
        _logger = logger;
        _docker = docker;
        _imageHub = imageHub;
    }

    public async Task<StartBuildResponse> Handle(StartBuildRequest request, CancellationToken cancellationToken)
    {
        await _docker.System.PingAsync(cancellationToken);

        var imageName = request.FileName.ToLower().Replace('.', '-');
        var buildParams = new ImageBuildParameters
        {
            Dockerfile = "Dockerfile",
            Tags = new List<string> { imageName },
        };

        var dockerProgress = new Progress<JSONMessage>();

        dockerProgress.ProgressChanged += async (_, e) =>
        {
            _logger.LogInformation("Progress: {@message}", e);
            await _imageHub.Clients.Client(request.SenderId).SendAsync("BuildProgress", e.Stream, CancellationToken.None);
        };

        Task.Run(async () =>
        {
            try
            {
                await _docker.Images.BuildImageFromDockerfileAsync(
                    buildParams,
                    request.File,
                    null,
                    null,
                    dockerProgress,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while building app");
            }
            finally
            {
                await request.File.DisposeAsync();
            }
        });

        return new StartBuildResponse(imageName, "latest");
    }
}

public record StartBuildResponse(string ImageName, string Tag);
public record StartBuildRequest(string SenderId, string FileName, Stream File) : IRequest<StartBuildResponse>;
