using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GameDock.Server.Application.Handlers;
using GameDock.Server.Domain.Enums;
using GameDock.Shared.Requests;
using GameDock.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameDock.Server.Controllers.Api;

[ApiController]
[Route("api/build")]
public class BuildController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(UploadResponse), 200)]
    public async Task<IActionResult> Upload(
        [FromForm] IFormFile archive,
        [FromForm] UploadRequest details,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var fileName = Path.GetFileName(archive.FileName);
        var tempFile = Path.Combine(Path.GetTempPath(), fileName);

        try
        {
            await using (var fileStream = new FileStream(tempFile, FileMode.Append))
            {
                await archive.CopyToAsync(fileStream, cancellationToken);
            }

            if (Request.Headers.TryGetValue("Chunk-Number", out var num)
                && Request.Headers.TryGetValue("Total-Chunks", out var total)
                && num != total)
                return Ok();
        }
        catch (TaskCanceledException)
        {
            System.IO.File.Delete(tempFile);
        }

        var fileType = Path.GetExtension(archive.FileName) switch
        {
            ".tar" => BuildArchiveType.Tar,
            ".zip" => BuildArchiveType.Zip,
            _ => throw new ArgumentOutOfRangeException("fileType"),
        };

        await using var resultFileStream = System.IO.File.OpenRead(tempFile);
        var info = await mediator.Send(
            new SaveBuildRequest(details.BuildName, details.Version, fileType, resultFileStream),
            cancellationToken);

        await mediator.Send(new StartImageBuildRequest(info.Id, details.RuntimePah, details.LaunchParameters),
            cancellationToken);

        System.IO.File.Delete(tempFile);

        return Ok(new UploadResponse
        {
            Id = info.Id,
            Status = info.Status.ToString(),
        });
    }
}