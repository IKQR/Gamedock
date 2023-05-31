using GameDock.Server.Application.Services;
using GameDock.Server.Domain.Build;
using GameDock.Server.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Application.Handlers;

public class SaveBuildRequestHandler : IRequestHandler<SaveBuildRequest, BuildInfo>
{
    private readonly ILogger _logger;
    private readonly IBuildInfoRepository _infos;
    private readonly IBuildFileRepository _files;
    private readonly ITransactionManager _transaction;

    public SaveBuildRequestHandler(ILogger<SaveBuildRequestHandler> logger, IBuildInfoRepository infos,
        IBuildFileRepository files, ITransactionManager transaction)
    {
        _logger = logger;
        _infos = infos;
        _files = files;
        _transaction = transaction;
    }

    public async Task<BuildInfo> Handle(SaveBuildRequest request, CancellationToken cancellationToken)
    {
        var result = await _transaction.InTransactionAsync(async token =>
        {
            var info = await _infos.AddAsync(request.Name, request.Version, request.Path, cancellationToken);

            await _files.SaveAsync(info.Id.ToString(), request.Stream, token);

            return info;
        }, cancellationToken);

        return result;
    }
}

public record SaveBuildRequest(string Name, string Version, string Path, Stream Stream, BuildArchiveType Type)
    : IRequest<BuildInfo>;