using System;
using System.Threading;
using System.Threading.Tasks;
using GameDock.Server.Application.Handlers;
using GameDock.Server.Domain.Build;
using GameDock.Server.Domain.Enums;
using GameDock.Shared.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameDock.Server.Controllers.Api;

[ApiController]
[Route("api/build")]
public class BuildController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(BuildInfo), 200)]
    public async Task<IActionResult> Upload(
        [FromForm] IFormFile archive,
        [FromForm] UploadRequest details,
        [FromServices] IMediator mediator,
        [FromServices] CancellationToken cancellationToken)
    {
        var fileType = System.IO.Path.GetExtension(archive.FileName) switch
        {
            ".tar" => BuildArchiveType.Tar,
            ".zip" => BuildArchiveType.Zip,
            _ => throw new ArgumentOutOfRangeException("fileType"),
        };

        await using var fileStream = archive.OpenReadStream();
        var info = await mediator.Send(new SaveBuildRequest(details.BuildName, details.Version, fileType, fileStream),
            cancellationToken);

        await mediator.Send(new StartImageBuildRequest(info.Id, "DotnetGameServer", ""), cancellationToken);

        return Ok(info);
    }
}