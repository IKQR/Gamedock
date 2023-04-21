using System.Threading;
using System.Threading.Tasks;
using GameDock.Application.Handlers;
using GameDock.Domain.Enums;
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
    public async Task<IActionResult> AddBuild(string buildName, string version, BuildArchiveType type, IFormFile archive,
        CancellationToken cancellationToken)
    {
        await using var fileStream = archive.OpenReadStream();
        await _mediator.Send(new SaveBuildRequest(buildName, version, type, fileStream), cancellationToken);

        return Ok();
    }
}