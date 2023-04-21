using GameDock.Application.Helpers;
using GameDock.Application.Services;
using GameDock.Domain.Build;
using GameDock.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameDock.Application.Handlers;

public class SaveBuildRequestHandler : IRequestHandler<SaveBuildRequest, BuildInfo>
{
    private readonly ILogger _logger;
    private readonly IBuildInfoRepository _infos;
    private readonly IBuildFileRepository _files;

    public SaveBuildRequestHandler(ILogger<SaveBuildRequestHandler> logger, IBuildInfoRepository infos,
        IBuildFileRepository files)
    {
        _logger = logger;
        _infos = infos;
        _files = files;
    }

    public async Task<BuildInfo> Handle(SaveBuildRequest request, CancellationToken cancellationToken)
    {
        var archive = request.Archive;
        if (request.Type is BuildArchiveType.Zip)
        {
            archive = await ArchiveHelper.ZipToTarAsync(archive, true);
        }

        var key = await _files.Add(archive, cancellationToken);

        try
        {
            var buildInfo = new BuildInfo(key, request.BuildName, request.Version, BuildStatus.Saved);
            await _infos.Add(buildInfo);

            return buildInfo;
        }
        catch (Exception)
        {
            await _files.TryRemove(key, CancellationToken.None);
            throw;
        }
        finally
        {
            await archive.DisposeAsync();
        }
    }
}

public record SaveBuildRequest
    (string BuildName, string Version, BuildArchiveType Type, Stream Archive) : IRequest<BuildInfo>;