using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Application.Handlers.Sessions;

public class StopSessionHandler : IRequestHandler<StopSessionRequest>
{
    private readonly ILogger _logger;
    private readonly ISessionManager _sessionManager;
    private readonly ISessionInfoRepository _sessionInfoRepository;

    public StopSessionHandler(ILogger<StopSessionHandler> logger, ISessionManager sessionManager,
        ISessionInfoRepository sessionInfoRepository)
    {
        _logger = logger;
        _sessionManager = sessionManager;
        _sessionInfoRepository = sessionInfoRepository;
    }

    public async Task Handle(StopSessionRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stop session requested. Container {ContainerId}", request.ContainerId);

        var session = await _sessionInfoRepository.GetByIdAsync(request.ContainerId, cancellationToken);

        if (session?.Status is not SessionStatus.Running)
        {
            return;
        }

        var result = await _sessionManager.StopAsync(session.ContainerId, cancellationToken: cancellationToken);

        if (result)
        {
            await _sessionInfoRepository.SetAsync(session with { Status = SessionStatus.Stopped },
                CancellationToken.None);
            return;
        }

        var info = await _sessionManager.GetStatusAsync(session.ContainerId, cancellationToken);

        await _sessionInfoRepository.SetAsync(info ?? session with { Status = SessionStatus.Undefined },
            CancellationToken.None);
    }
}

public record StopSessionRequest(string ContainerId) : IRequest;