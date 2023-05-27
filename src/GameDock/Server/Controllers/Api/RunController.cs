using System;
using System.Threading.Tasks;
using GameDock.Server.Application.Handlers;
using GameDock.Server.Mappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameDock.Server.Controllers.Api;

[ApiController]
public class RunController : ControllerBase
{
    [HttpPost("/api/build/{buildId:required}/run")]
    public async Task<IActionResult> Run([FromRoute] Guid buildId, [FromServices]IMediator mediator)
    {
        var result = await mediator.Send(new RunSessionRequest(buildId));
        var mappedResult = SessionInfoMapper.Map(result);
        mappedResult.Ip = HttpContext.Request.Host.Host;
        return Ok(mappedResult);
    }
}