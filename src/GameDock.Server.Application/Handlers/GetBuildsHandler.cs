using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Build;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Application.Handlers;

public class GetBuildsHandler : IRequestHandler<GetBuildsRequest, IList<BuildInfo>>
{
    private readonly ILogger _logger;
    private readonly IBuildInfoRepository _infos;

    public GetBuildsHandler(ILogger<GetBuildsHandler> logger, IBuildInfoRepository infos)
    {
        _logger = logger;
        _infos = infos;
    }

    public async Task<IList<BuildInfo>> Handle(GetBuildsRequest request, CancellationToken cancellationToken)
    {
        var getAll = !request.Ids.Any();

        _logger.LogInformation("Build info requested for [{Ids}]", getAll ? "All" : string.Join(", ", request.Ids));

        return getAll
            ? await _infos.GetAllAsync(cancellationToken)
            : await _infos.GetByIdAsync(request.Ids, cancellationToken);
    }
}

public record GetBuildsRequest(params Guid[] Ids) : IRequest<IList<BuildInfo>>;