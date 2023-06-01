using System.Diagnostics.CodeAnalysis;
using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Application.Handlers.Builds;

public class GetFileStreamHandler : IRequestHandler<GetFileStreamRequest, Stream>
{
    private readonly ILogger<GetFileStreamHandler> _logger;
    private readonly IBuildInfoRepository _infos;
    private readonly IBuildFileRepository _files;

    public GetFileStreamHandler(ILogger<GetFileStreamHandler> logger, IBuildInfoRepository infos,
        IBuildFileRepository files)
    {
        _logger = logger;
        _infos = infos;
        _files = files;
    }
    
    public async Task<Stream> Handle(GetFileStreamRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Build file download requested. Build {BuildId}", request.Id);

        var info = await _infos.GetByIdAsync(request.Id, cancellationToken);

        if (info.Status is BuildStatus.Deleted)
        {
            return null;
        }

        var stream = await _files.GetStreamReadAsync(info.Id.ToString(), cancellationToken);

        return stream;
    }
}

public record GetFileStreamRequest(Guid Id) : IRequest<Stream>;