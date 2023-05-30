using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameDock.Server.Application.Handlers.Fleets;
using GameDock.Server.Mappers;
using GameDock.Shared.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameDock.Server.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/fleet")]
public class FleetController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(FleetInfoDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> Index([FromServices] IMediator mediator, CancellationToken cancellationToken,
        [FromBody] CreateFleetDto request)
    {
        var result = await mediator.Send(
            new CreateFleetRequest(request.BuildId, request.Runtime, request.Ports,
                request.LaunchParameters ?? string.Empty,
                request.Variables ?? new Dictionary<string, string>()), cancellationToken);

        if (result.IsSuccessful)
        {
            return Ok(FleetInfoMapper.Map(result.FleetInfo));
        }

        return BadRequest(result.Error);
    }
}