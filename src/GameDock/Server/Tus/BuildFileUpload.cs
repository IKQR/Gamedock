using System;
using System.IO;
using GameDock.Server.Application.Handlers;
using GameDock.Server.Domain.Enums;
using GameDock.Server.Utils;
using GameDock.Shared;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using tusdotnet;

namespace GameDock.Server.TUS;

public static class BuildFileUpload
{
    public static IEndpointConventionBuilder MapBuildUpload(this IEndpointRouteBuilder builder) =>
        builder.MapTus("/api/build/upload", async _ => new()
        {
            Store = new tusdotnet.Stores.TusDiskStore(Path.GetTempPath()),
            Events = new()
            {
                OnFileCompleteAsync = async eventContext =>
                {
                    var cancellationToken = eventContext.CancellationToken;

                    var file = await eventContext.GetFileAsync();
                    var metadata = await file.GetMetadataAsync(cancellationToken);
                    await using var fileStream = await file.GetContentAsync(cancellationToken);

                    var buildInfo = metadata.Deserialize<BuildMetadata>();

                    var fileType = Path.GetExtension(buildInfo.FileName) switch
                    {
                        ".tar" => BuildArchiveType.Tar,
                        // ".zip" => BuildArchiveType.Zip,
                        _ => throw new ArgumentOutOfRangeException("fileType"),
                    };

                    var request = new SaveBuildRequest(buildInfo.BuildName, buildInfo.Version, buildInfo.RuntimePah,
                        fileStream, fileType);

                    await eventContext.HttpContext.RequestServices.GetRequiredService<IMediator>()
                        .Send(request, cancellationToken);
                },
            },
        });
}