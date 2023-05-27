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
    [ProducesResponseType(400)]
    [ProducesResponseType(typeof(BuildInfoDto[]), 200)]
    public async Task<IActionResult> GetAll(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var info = await mediator.Send(new GetBuildsRequest(), cancellationToken);

        return Ok(info.Select(BuildInfoMapper.Map));
    }

    [HttpGet]
    [Route("{id:required}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(BuildInfoDto), 200)]
    public async Task<IActionResult> Get(
        [FromRoute] string id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var guidId))
        {
            return BadRequest();
        }

        var info = await mediator.Send(new GetBuildsRequest(guidId), cancellationToken);

        if (!info.Any())
        {
            return NotFound();
        }

        return Ok(BuildInfoMapper.Map(info.Single()));
    }
}