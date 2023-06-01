using System;
using System.Collections.Generic;
using System.Linq;
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
[Route("/api/fleet/info")]
public class FleetInfoController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FleetInfoDto>), 200)]
    public async Task<IActionResult> GetAll([FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var results = await mediator.Send(new GetFleetsRequest(), cancellationToken);

        return Ok(!results.Any() ? Array.Empty<FleetInfoDto>() : results.Select(FleetInfoMapper.Map));
    }

    [HttpGet]
    [Route("{id:required}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(FleetInfoDto), 200)]
    public async Task<IActionResult> Get([FromServices] IMediator mediator, CancellationToken cancellationToken, 
        [FromRoute] string id)
    {
        if (!Guid.TryParse(id, out var guidId))
        {
            return BadRequest();
        }

        var info = await mediator.Send(new GetFleetsRequest(Id: guidId), cancellationToken);

        if (!info.Any())
        {
            return NotFound();
        }

        return Ok(FleetInfoMapper.Map(info.Single()));
    }
}