using GameDock.Server.Application.Services;
using GameDock.Server.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Application.Handlers.Sessions;

public class FindSessionsHandler : IRequestHandler<FindSessionsRequest, IList<SessionInfo>>
{
    private readonly ILogger _logger;
    private readonly ISessionManager _sessionManager;
    private readonly IFleetInfoRepository _fleetInfoRepository;
    private readonly ISessionInfoRepository _sessionInfoRepository;

    public FindSessionsHandler(ILogger<FindSessionsHandler> logger, ISessionManager sessionManager,
        IFleetInfoRepository fleetInfoRepository, ISessionInfoRepository sessionInfoRepository)
    {
        _logger = logger;
        _sessionManager = sessionManager;
        _sessionInfoRepository = sessionInfoRepository;
    }

    public async Task<IList<SessionInfo>> Handle(FindSessionsRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Id is not null)
        {
            var status = await _sessionManager.GetStatusAsync(request.Id, cancellationToken);
            await _sessionInfoRepository.SetAsync(status, CancellationToken.None);
            return new[] { status };
        }

        if (request.FleetId is null)
        {
            return Array.Empty<SessionInfo>();
        }

        var fleet = await _fleetInfoRepository.GetByIdAsync((Guid)request.FleetId, cancellationToken);

        if (fleet is null)
        {
            return Array.Empty<SessionInfo>();
        }

        var statuses = await _sessionManager.GetGroupStatusAsync(fleet.ImageKey, cancellationToken);
        await _sessionInfoRepository.SetManyAsync(statuses, CancellationToken.None);

        return statuses;
    }
}

public record FindSessionsRequest(string Id = null, Guid? FleetId = null) : IRequest<IList<SessionInfo>>;