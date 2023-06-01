using GameDock.Server.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Application.Handlers.Builds;

public class DeleteBuildHandler : IRequestHandler<DeleteBuildRequest>
{
    private readonly ILogger<DeleteBuildHandler> _logger;
    private readonly IBuildInfoRepository _infos;
    private readonly IBuildFileRepository _files;

    public DeleteBuildHandler(ILogger<DeleteBuildHandler> logger, IBuildInfoRepository infos,
        IBuildFileRepository files)
    {
        _logger = logger;
        _infos = infos;
        _files = files;
    }

    public async Task Handle(DeleteBuildRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Build delete requested. Build {BuildId}", request.Id);

        var isRemoved = await _files.TryDeleteAsync(request.Id.ToString(), cancellationToken);
        var isDeleted = await _infos.TrySetDeleted(request.Id, CancellationToken.None);

        if (!isRemoved || !isDeleted)
        {
            _logger.LogWarning(
                "Strange actions while deleting build. File deleted ({FileDeleted}). Info deleted ({BuildDeleted})",
                isRemoved, isDeleted);
        }
    }
}

public record DeleteBuildRequest(Guid Id) : IRequest;