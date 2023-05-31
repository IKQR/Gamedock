using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Enums;
using GameDock.Server.Domain.Fleet;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Application.Handlers.Fleets;

public class CreateFleetHandler : IRequestHandler<CreateFleetRequest, CreateFleetResponse>
{
    private readonly ILogger _logger;
    private readonly IBuildInfoRepository _builds;
    private readonly IFleetInfoRepository _fleets;

    public CreateFleetHandler(ILogger<CreateFleetHandler> logger, IBuildInfoRepository builds,
        IFleetInfoRepository fleets)
    {
        _logger = logger;
        _builds = builds;
        _fleets = fleets;
    }

    public async Task<CreateFleetResponse> Handle(CreateFleetRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create fleet requested. Request: {@Request}", request);

        var build = await _builds.GetByIdAsync(request.BuildId, cancellationToken);

        if (build is null)
        {
            return CreateFleetResponse.Failed("Build not found");
        }

        if (build.Status is not BuildStatus.Saved)
        {
            return CreateFleetResponse.Failed("Build not ready");
        }

        var newFleet = await _fleets.AddAsync(build.Id, request.Runtime, request.Ports, request.Parameters,
            request.Variables, cancellationToken);

        _logger.LogInformation("Fleet successfully created. Fleet: {@FleetId}", newFleet.Id);

        return CreateFleetResponse.Successful(newFleet);
    }
}

public sealed record CreateFleetRequest(Guid BuildId, string Runtime, int[] Ports, string Parameters,
    IDictionary<string, string> Variables) : IRequest<CreateFleetResponse>;

public record CreateFleetResponse(bool IsSuccessful, FleetInfo FleetInfo = null, string Error = null)
{
    public static CreateFleetResponse Successful(FleetInfo info) => new(true, info);
    public static CreateFleetResponse Failed(string error) => new(false, Error: error);
}