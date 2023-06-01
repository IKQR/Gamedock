using GameDock.Server.Application.Services;
using GameDock.Server.Domain;
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
        _logger.LogInformation("Build info requested. Request: {@Request}", request);

        if (request.Id is not null)
        {
            return new List<BuildInfo>
            {
                await _infos.GetByIdAsync(request.Id.Value, cancellationToken),
            };
        }

        if (request.Name is not null)
        {
            return await _infos.GetByNameAsync(request.Name, request.Version, cancellationToken);
        }

        return await _infos.GetAllAsync(cancellationToken);
    }
}

public record GetBuildsRequest(Guid? Id = null, string Name = null, string Version = null) : IRequest<IList<BuildInfo>>;