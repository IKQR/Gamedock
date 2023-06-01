using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Fleet;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Application.Handlers.Fleets;

public class GetFleetsHandler : IRequestHandler<GetFleetsRequest, IList<FleetInfo>>
{
    private readonly ILogger _logger;
    private readonly IFleetInfoRepository _infos;

    public GetFleetsHandler(ILogger<GetFleetsHandler> logger, IFleetInfoRepository infos)
    {
        _logger = logger;
        _infos = infos;
    }

    public async Task<IList<FleetInfo>> Handle(GetFleetsRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fleet info requested. Request: {@Request}", request);

        if (request.Id is null)
        {
            return await _infos.GetAllAsync(cancellationToken);
        }
        
        var result = await _infos.GetByIdAsync(request.Id.Value, cancellationToken);

        return result is not null ? new List<FleetInfo> { result } : new List<FleetInfo>();
    }
}

public record GetFleetsRequest(Guid? Id = null) : IRequest<IList<FleetInfo>>;