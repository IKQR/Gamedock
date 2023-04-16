using System;
using System.IO;
using System.Threading.Tasks;
using GameDock.Server.Handlers;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameDock.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IMediator _mediator;

    public FileController(IWebHostEnvironment environment, IMediator mediator)
    {
        _environment = environment;
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), 200)]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var code = Guid.NewGuid();
        var filePath = Path.Combine(_environment.ContentRootPath, "Uploads", code.ToString());
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Ok(code);
    }
}