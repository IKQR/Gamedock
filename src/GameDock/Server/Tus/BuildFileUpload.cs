using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using tusdotnet;

namespace GameDock.Server.TUS;

public static class BuildFileUpload
{
    public static TApplication MapBuildUpload<TApplication>(this TApplication app, IConfiguration configuration)
        where TApplication : IEndpointRouteBuilder
    {
        app.MapTus("/api/build/upload", async _ => new()
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

                    var logger = eventContext.HttpContext.RequestServices.GetService<ILogger<Program>>();
                    logger.LogInformation("File uploaded. File: '{Filename}'. Metadata: {@Meta}", file.Id, metadata);
                },
            },
        });

        return app;
    }
}