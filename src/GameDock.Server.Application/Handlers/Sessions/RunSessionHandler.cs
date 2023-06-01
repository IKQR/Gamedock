using GameDock.Server.Application.Services;
using GameDock.Server.Domain;
using GameDock.Server.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Application.Handlers.Sessions;

public class RunSessionHandler : IRequestHandler<RunSessionRequest, RunSessionResponse>
{
    private readonly ILogger _logger;
    private readonly ISessionManager _sessionManager;
    private readonly IFleetInfoRepository _fleetInfoRepository;
    private readonly ISessionInfoRepository _sessionInfoRepository;

    public RunSessionHandler(ILogger<RunSessionHandler> logger, ISessionManager sessionManager,
        IFleetInfoRepository fleetInfoRepository, ISessionInfoRepository sessionInfoRepository)
    {
        _logger = logger;
        _sessionManager = sessionManager;
        _fleetInfoRepository = fleetInfoRepository;
        _sessionInfoRepository = sessionInfoRepository;
    }

    public async Task<RunSessionResponse> Handle(RunSessionRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Run session requested for fleet '{FleetId}'", request.FleetId);

        var fleet = await _fleetInfoRepository.GetByIdAsync(request.FleetId, cancellationToken);

        if (fleet is null)
        {
            return new RunSessionResponse(false, Error: "Fleet not found.");
        }

        if (fleet.Status is not FleetStatus.Ready)
        {
            return new RunSessionResponse(false, Error: "Fleet not ready");
        }

        var session = await _sessionManager.RunAsync(fleet.ImageKey, fleet.Ports, cancellationToken);

        await _sessionInfoRepository.SetAsync(session, CancellationToken.None);

        _logger.LogInformation("Session successfully run. Session: '{SessionId}'", session.ContainerId);

        return new RunSessionResponse(true, session);
    }
}

public record RunSessionRequest(Guid FleetId, IDictionary<string, string> Variables) : IRequest<RunSessionResponse>;

public record RunSessionResponse(bool IsSuccessful, SessionInfo Session = null, string Error = null);