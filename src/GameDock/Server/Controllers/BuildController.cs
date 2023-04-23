using System;
using System.Threading;
using System.Threading.Tasks;
using GameDock.Server.Application.Handlers;
using GameDock.Server.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameDock.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuildController : ControllerBase
{
    private readonly IMediator _mediator;

    public BuildController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddBuild(string buildName, string version, IFormFile archive, CancellationToken cancellationToken)
    {
        var fileType = System.IO.Path.GetExtension(archive.FileName) switch
        {
            ".tar" => BuildArchiveType.Tar,
            ".zip" => BuildArchiveType.Zip,
            _ => throw new ArgumentOutOfRangeException("fileType"),
        };
        
        await using var fileStream = archive.OpenReadStream();
        var info = await _mediator.Send(new SaveBuildRequest(buildName, version, fileType, fileStream), cancellationToken);

        return Ok(info);
    }
}