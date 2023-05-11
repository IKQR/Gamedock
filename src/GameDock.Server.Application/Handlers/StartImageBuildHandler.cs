using GameDock.Server.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Application.Handlers;

public class StartImageBuildHandler : IRequestHandler<StartImageBuildRequest>
{
    private readonly ILogger _logger;
    private readonly IImageBuilder _imageBuilder;

    public StartImageBuildHandler(ILogger<StartImageBuildHandler> logger, IImageBuilder imageBuilder)
    {
        _logger = logger;
        _imageBuilder = imageBuilder;
    }

    public Task Handle(StartImageBuildRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Preparing for game server '{Id}' image build", request.BuildId);

        Task.Run(() =>
                _imageBuilder.BuildImageFromArchive(request.BuildId, request.RuntimePath, request.LaunchParameters,
                    CancellationToken.None),
            CancellationToken.None);

        _logger.LogInformation("Build '{Id}' scheduled", request.BuildId);

        return Task.CompletedTask;
    }
}

public record StartImageBuildRequest(Guid BuildId, string RuntimePath, string LaunchParameters) : IRequest;