using System;
using System.IO;
using GameDock.Server.Application.Handlers;
using GameDock.Server.Domain.Enums;
using GameDock.Server.Utils;
using GameDock.Shared;
using Hangfire.Logging;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using tusdotnet;
using tusdotnet.Interfaces;
using tusdotnet.Models.Expiration;

namespace GameDock.Server.TUS;

public static class BuildFileUpload
{
    public static IEndpointConventionBuilder MapBuildUpload(this IEndpointRouteBuilder builder) =>
        builder.MapTus("/api/build/upload", async _ => new()
        {
            MaxAllowedUploadSizeInBytesLong = long.MaxValue,
            Store = new tusdotnet.Stores.TusDiskStore(Path.GetTempPath()),
            Expiration = new SlidingExpiration(TimeSpan.FromHours(1)),
            Events = new()
            {
                OnFileCompleteAsync = async ctx =>
                {
                    var cancellationToken = ctx.CancellationToken;

                    var file = await ctx.GetFileAsync();
                    var metadata = await file.GetMetadataAsync(cancellationToken);
                    var fileStream = await file.GetContentAsync(cancellationToken);

                    try
                    {
                        var buildInfo = metadata.Deserialize<BuildMetadata>();

                        var fileType = Path.GetExtension(buildInfo.FileName) switch
                        {
                            ".tar" => BuildArchiveType.Tar,
                            _ => throw new ArgumentOutOfRangeException("fileType"),
                        };

                        var request = new SaveBuildRequest(buildInfo.BuildName, buildInfo.Version,
                            buildInfo.RuntimePah,
                            fileStream, fileType);

                        await ctx.HttpContext.RequestServices.GetRequiredService<IMediator>()
                            .Send(request, cancellationToken);
                    }
                    finally
                    {
                        await fileStream.DisposeAsync();

                        var store = (ITusTerminationStore)ctx.Store;
                        await store.DeleteFileAsync(file.Id, cancellationToken);
                    }
                },
            },
        });
}