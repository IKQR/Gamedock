using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameDock.Server.Application.Handlers;
using GameDock.Server.Domain.Build;
using GameDock.Server.Mappers;
using GameDock.Shared.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameDock.Server.Controllers.Api;

[ApiController]
[Route("api/build/info")]
public class BuildInfoController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(IEnumerable<BuildInfoDto>), 200)]
    public async Task<IActionResult> GetAll([FromServices] IMediator mediator, CancellationToken cancellationToken,
        [FromQuery] string name = null, [FromQuery] string version = null)
    {
        var results = await mediator.Send(new GetBuildsRequest(Name: name, Version: version), cancellationToken);

        if (!results.Any())
        {
            return NoContent();
        }

        return Ok(results.Select(BuildInfoMapper.Map));
    }

    [HttpGet]
    [Route("{id:required}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(BuildInfoDto), 200)]
    public async Task<IActionResult> Get([FromServices] IMediator mediator, CancellationToken cancellationToken,
        [FromRoute] string id)
    {
        if (!Guid.TryParse(id, out var guidId))
        {
            return BadRequest();
        }

        var info = await mediator.Send(new GetBuildsRequest(Id: guidId), cancellationToken);

        if (!info.Any())
        {
            return NotFound();
        }

        return Ok(BuildInfoMapper.Map(info.Single()));
    }
}