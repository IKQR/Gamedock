using System;
using System.Threading;
using System.Threading.Tasks;
using GameDock.Server.Application.Handlers.Sessions;
using GameDock.Shared.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameDock.Server.Controllers.Api;

[ApiController]
[Route("api/fleet/{fleetId:required}/session")]
public class SessionController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Run([FromServices] IMediator mediator, CancellationToken cancellationToken,
        [FromRoute] string fleetId, [FromBody]RunSessionDto request )
    {
        if (!Guid.TryParse(fleetId, out var guidId))
        {
            return BadRequest("invalid fleet id format");
        }
        
        var result = await mediator.Send(new RunSessionRequest(guidId, request.Values), cancellationToken);

        if (!result.IsSuccessful)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Session);
    }
}