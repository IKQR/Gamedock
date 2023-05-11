using GameDock.Server.Application.Services;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Application.Handlers;

public class StartImageBuildHandler : IRequestHandler<StartImageBuildRequest>
{
    private readonly ILogger _logger;
    private readonly IImageBuilder _imageBuilder;
    private readonly IBackgroundJobClient _jobClient;

    public StartImageBuildHandler(ILogger<StartImageBuildHandler> logger, IImageBuilder imageBuilder,
        IBackgroundJobClient jobClient)
    {
        _logger = logger;
        _imageBuilder = imageBuilder;
        _jobClient = jobClient;
    }

    public Task Handle(StartImageBuildRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Preparing for game server '{Id}' image build", request.BuildId);

        _jobClient.Schedule(
            () => _imageBuilder.BuildImageFromArchive(
                request.BuildId,
                request.RuntimePath,
                request.LaunchParameters,
                CancellationToken.None),
            DateTimeOffset.UtcNow + TimeSpan.FromSeconds(5));


        _logger.LogInformation("Build '{Id}' scheduled", request.BuildId);

        return Task.CompletedTask;
    }
}

public record StartImageBuildRequest(Guid BuildId, string RuntimePath, string LaunchParameters) : IRequest;