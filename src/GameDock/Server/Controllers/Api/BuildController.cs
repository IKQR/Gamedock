using System;
using System.Threading;
using System.Threading.Tasks;
using GameDock.Server.Application.Handlers.Builds;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameDock.Server.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/build")]
public class BuildController : ControllerBase
{
    [HttpDelete("{id:required}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetAll([FromServices] IMediator mediator, CancellationToken cancellationToken,
        [FromRoute] string id)
    {
        if (!Guid.TryParse(id, out var guidId))
        {
            return BadRequest("Invalid id format");
        }

        await mediator.Send(new DeleteBuildRequest(guidId), cancellationToken);

        return Ok();
    }
    
    [HttpGet("{id:required}/download")]
    [ProducesResponseType(typeof(FileContentResult),200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DownloadFile([FromServices] IMediator mediator, CancellationToken cancellationToken,
        [FromRoute] string id)
    {
        if (!Guid.TryParse(id, out var guidId))
        {
            return BadRequest("Invalid id format");
        }

        var stream = await mediator.Send(new GetFileStreamRequest(guidId), cancellationToken);

        return File(stream, "application/x-tar",$"{id}.tar");
    }
}