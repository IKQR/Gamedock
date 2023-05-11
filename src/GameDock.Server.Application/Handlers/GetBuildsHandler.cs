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
        _logger.LogInformation("Build info requested for [{Ids}]",
            request.Ids.Any() ? string.Join(", ", request.Ids) : "All");

        var result = await _infos.GetByIdAsync(request.Ids, cancellationToken);

        return result;
    }
}

public record GetBuildsRequest(params Guid[] Ids) : IRequest<IList<BuildInfo>>;