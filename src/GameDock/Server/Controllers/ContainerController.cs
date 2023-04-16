using System;
using System.Threading.Tasks;
using GameDock.Server.Handlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameDock.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContainerController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContainerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Start([FromBody]string imageName)
    {
        await _mediator.Send(new StartContainerRequest(imageName, $"test-{DateTime.UtcNow:MM-dd-yyyy-HH-mm}"));
        return Ok();
    }
}