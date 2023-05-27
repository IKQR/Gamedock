using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Session;
using MediatR;

namespace GameDock.Server.Application.Handlers;

public class RunSessionHandler : IRequestHandler<RunSessionRequest, SessionInfo>
{
    private ISessionRunner _runner;

    public RunSessionHandler(ISessionRunner runner)
    {
        _runner = runner;
    }

    public async Task<SessionInfo> Handle(RunSessionRequest request, CancellationToken cancellationToken)
    {
        var session = await _runner.RunSessionFromBuild(request.Id, cancellationToken: cancellationToken);
        
        return session;
    }
}

public record RunSessionRequest(Guid Id) : IRequest<SessionInfo>;