using System.Transactions;
using GameDock.Application.Helpers;
using GameDock.Application.Services;
using GameDock.Server.Domain.Build;
using GameDock.Server.Domain.Enums;
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

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var info = await _infos.AddAsync(request.BuildName, request.Version, cancellationToken);

            await _files.Add(info.Id.ToString(), archive, cancellationToken);

            return info;
        }
    }
}

public record SaveBuildRequest(string BuildName, string Version, BuildArchiveType Type, Stream Archive)
    : IRequest<BuildInfo>;